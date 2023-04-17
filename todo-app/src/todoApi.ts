import { TodoList } from "./todoTypes";

export class TodoApi {
  public readonly apiUrl: string = process.env.REACT_APP_TODO_API_URL as string;

  private replaceNullsWithEmptyString(data: TodoList[]) {
    data.forEach((todoList) => {
      if (todoList.name == null) todoList.name = "";
      todoList.todos.forEach((todoItem) => {
        if (todoItem.description == null) todoItem.description = "";
      });
    });
  }

  async getTodoListsAsync(): Promise<TodoList[]> {
    try {
      let response = await fetch(this.apiUrl);
      let data = await response.json();
      this.replaceNullsWithEmptyString(data);
      return data;
    } catch (error) {
      console.error(error);
      throw error;
    }
  }

  async upsertTodoListAsync(modifiedList: TodoList): Promise<number> {
    try {
      if (modifiedList.id === 0) {
        return await this.postTodoListAsync(modifiedList);
      } else {
        this.patchTodoListAsync(modifiedList);
        return modifiedList.id;
      }
    } catch (error) {
      console.error(error);
      throw error;
    }
  }

  async deleteTodoListAsync(id: number) {
    try {
      await fetch(this.apiUrl + `?id=${id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json; charset=UTF-8",
        },
      });
    } catch (error) {
      console.error(error);
      throw error;
    }
  }

  private async patchTodoListAsync(modifiedList: TodoList) {
    this.replaceNullsWithEmptyString([modifiedList]);
    await fetch(this.apiUrl, {
      method: "PATCH",
      headers: {
        "Content-Type": "application/json; charset=UTF-8",
      },
      body: JSON.stringify(modifiedList),
    });
  }

  private async postTodoListAsync(modifiedList: TodoList): Promise<number> {
    this.replaceNullsWithEmptyString([modifiedList]);
    let result = await fetch(this.apiUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=UTF-8",
      },
      body: JSON.stringify(modifiedList),
    });
    return parseInt(await result.text());
  }
}
