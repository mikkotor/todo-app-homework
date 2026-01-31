import { validate } from "uuid";
import { TodoList } from "./todoTypes";

export class TodoApi {
  public readonly apiBaseUrl: string = import.meta.env.VITE_TODO_API_URL ?? "";

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
      let response = await this.callApi(this.apiBaseUrl, "GET", token);
      if (!response.ok) throw new Error(`Failed to get todo lists`);
      let data = await response.json();
      this.replaceNullsWithEmptyString(data);
      return data;
    } catch (error: any) {
      console.error(error);
      throw error;
    }
  }

  async upsertTodoListAsync(modifiedList: TodoList, token?: string): Promise<string> {
    try {
      if (!modifiedList.id) {
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

  async deleteTodoListAsync(id: string, token?: string) {
    try {
      let response = await this.callApi(`${this.apiBaseUrl}?id=${id}`, "DELETE", token);
      if (!response.ok) throw new Error(`Failed to delete list with id ${id}`);
    } catch (error: any) {
      console.error(error);
      throw error;
    }
  }

  private async patchTodoListAsync(modifiedList: TodoList, token?: string) {
    this.replaceNullsWithEmptyString([modifiedList]);
    let response = await this.callApi(
      this.apiBaseUrl,
      "PATCH",
      token,
      { "Content-Type": "application/json; charset=UTF-8" },
      JSON.stringify(modifiedList),
    );
    if (!response.ok) throw new Error(`Failed to update list: ${JSON.stringify(modifiedList)}`);
  }

  private async postTodoListAsync(modifiedList: TodoList, token?: string): Promise<string> {
    this.replaceNullsWithEmptyString([modifiedList]);
    let response = await this.callApi(
      this.apiBaseUrl,
      "POST",
      token,
      { "Content-Type": "application/json; charset=UTF-8" },
      JSON.stringify(modifiedList),
    );
    if (response.ok) {
      var responseString = await response.text();
      if (validate(responseString)) {
        return responseString;
      } else {
        throw new Error("Invalid UUID returned from server");
      }
    } else throw new Error("Failed to add a new todo list");
  }
}
