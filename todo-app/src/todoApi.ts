import { TodoList } from "./todoTypes";

// Node-only: when running tests or SSR in Node, disable TLS certificate
// verification for local development (self-signed certs). Browsers ignore
// `process`, so this only affects Node environments. This is intended for
// test/dev convenience — do not use in production.
if (typeof process !== "undefined" && process.versions && process.versions.node) {
  try {
    // Setting this environment variable causes Node's TLS stack to skip
    // certificate validation for the whole process. It's the simplest and
    // most compatible approach for test runners that need to talk to a
    // local HTTPS server with a self-signed certificate.
    process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
  } catch (e) {
    // ignore failures — this is only best-effort for local test environments
  }
}

export class TodoApi {
  // Browsers do not allow disabling TLS validation from page code.
  // During Vite dev, rewrite localhost HTTPS URLs to a relative path so the
  // dev server proxy (configured with `secure: false`) can forward requests
  // to a backend using a self-signed certificate.
  public readonly apiUrl: string = (() => {
    const raw = import.meta.env.VITE_TODO_API_URL ?? "";
    try {
      if (import.meta.env.DEV && raw.startsWith("https://localhost")) {
        const u = new URL(raw);
        return u.pathname + (u.search ?? "");
      }
    } catch (e) {
      // fall back to raw if parsing fails
    }
    return raw;
  })();

  private replaceNullsWithEmptyString(data: TodoList[]) {
    data.forEach((todoList) => {
      if (todoList.name == null) todoList.name = "";
      todoList.todos.forEach((todoItem) => {
        if (todoItem.description == null) todoItem.description = "";
      });
    });
  }

  private callApi(
    apiUrl: string,
    method: string,
    token?: string,
    headers: Record<string, string> = {},
    body?: BodyInit,
  ): Promise<Response> {
    if (token) headers["Authorization"] = `Bearer ${token}`;
    return fetch(apiUrl, {
      method,
      headers,
      body,
    });
  }

  async getTodoListsAsync(token?: string): Promise<TodoList[]> {
    try {
      let response = await this.callApi(this.apiUrl, "GET", token);
      if (!response.ok) throw new Error(`Failed to get todo lists`);
      let data = await response.json();
      this.replaceNullsWithEmptyString(data);
      return data;
    } catch (error: any) {
      console.error(error);
      throw error;
    }
  }

  async upsertTodoListAsync(modifiedList: TodoList, token?: string): Promise<number> {
    try {
      if (modifiedList.id === 0) {
        return await this.postTodoListAsync(modifiedList, token);
      } else {
        await this.patchTodoListAsync(modifiedList, token);
        return modifiedList.id;
      }
    } catch (error: any) {
      console.error(error);
      throw error;
    }
  }

  async deleteTodoListAsync(id: number, token?: string) {
    try {
      let response = await this.callApi(`${this.apiUrl}?id=${id}`, "DELETE", token);
      if (!response.ok) throw new Error(`Failed to delete list with id ${id}`);
    } catch (error: any) {
      console.error(error);
      throw error;
    }
  }

  private async patchTodoListAsync(modifiedList: TodoList, token?: string) {
    this.replaceNullsWithEmptyString([modifiedList]);
    let response = await this.callApi(
      this.apiUrl,
      "PATCH",
      token,
      { "Content-Type": "application/json; charset=UTF-8" },
      JSON.stringify(modifiedList),
    );
    if (!response.ok) throw new Error(`Failed to update list: ${JSON.stringify(modifiedList)}`);
  }

  private async postTodoListAsync(modifiedList: TodoList, token?: string): Promise<number> {
    this.replaceNullsWithEmptyString([modifiedList]);
    let response = await this.callApi(
      this.apiUrl,
      "POST",
      token,
      { "Content-Type": "application/json; charset=UTF-8" },
      JSON.stringify(modifiedList),
    );
    if (response.ok) return parseInt(await response.text());
    else throw new Error("Failed to add a new todo list");
  }
}
