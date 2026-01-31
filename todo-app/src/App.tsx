import React, { useState, useEffect } from "react";
// import logo from "./logo.svg";
import { useAuth0, AuthorizationParams } from "@auth0/auth0-react";
import LoginButton from "./LoginButton";
import LogoutButton from "./LogoutButton";
import Profile from "./Profile";
import "./App.css";
import { TodoList, Todo } from "./todoTypes";
import { TodoApi } from "./todoApi";
import { v4 as uuidv4 } from "uuid";

const todoApi = new TodoApi();

function App() {
  const { isAuthenticated, isLoading, error, getAccessTokenSilently } = useAuth0();
  const [todoLists, setTodoLists] = useState<TodoList[]>([]);
  const [apiError, setApiError] = useState<string>("");
  const logAndSetError = (error: any) => {
    console.error(error.toString());
    setApiError(error.toString());
  };

  useEffect(() => {
    const load = async () => {
      try {
        const token = await getAccessTokenSilently({
          authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
        });
        const data = await todoApi.getTodoListsAsync(token);
        setTodoLists(data);
      } catch (apiError) {
        logAndSetError(apiError);
      }
    };
    if (!isLoading && isAuthenticated) void load();
  }, [isAuthenticated, getAccessTokenSilently, isLoading]);

  if (isLoading) {
    return (
      <div className="app-container">
        <div className="loading-state">
          <div className="loading-text">Loading...</div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="app-container">
        <div className="error-state">
          <div className="error-title">Oops!</div>
          <div className="error-message">Something went wrong</div>
          <div className="error-sub-message">{error.message}</div>
        </div>
      </div>
    );
  }

  async function onAddNewListClick(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    let newTodoList: TodoList = {
      name: "New list",
      id: undefined,
      todos: [{ description: "First item", isDone: false }],
    };
    try {
      const token = isAuthenticated
        ? await getAccessTokenSilently({
            authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
          })
        : undefined;
      newTodoList.id = await todoApi.upsertTodoListAsync(newTodoList, token);
    } catch (apiError: any) {
      logAndSetError(apiError);
      return;
    }
    let modified = [...todoLists];
    modified.push(newTodoList);
    setTodoLists(modified);
  }

  async function onDeleteListClick(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    if (event.currentTarget.parentElement?.parentElement?.id === undefined) return;
    let databaseId: string = event.currentTarget.parentElement.parentElement.id;
    let modified = [...todoLists];
    var found = modified.findIndex((i) => i.id === databaseId);
    if (found === -1) return;
    modified.splice(found, 1);
    try {
      const token = isAuthenticated
        ? await getAccessTokenSilently({
            authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
          })
        : undefined;
      await todoApi.deleteTodoListAsync(databaseId, token);
    } catch (apiError: any) {
      logAndSetError(apiError);
      return;
    }
    setTodoLists(modified);
  }

  async function onItemDoneChange(event: React.ChangeEvent<HTMLInputElement>) {
    let elementTree = event.target.id.split(":");
    let modified = [...todoLists];
    modified[parseInt(elementTree[0])].todos[parseInt(elementTree[1])].isDone = event.target.checked;
    try {
      const token = isAuthenticated
        ? await getAccessTokenSilently({
            authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
          })
        : undefined;
      await todoApi.upsertTodoListAsync(modified[parseInt(elementTree[0])], token);
    } catch (apiError: any) {
      logAndSetError(apiError);
      return;
    }
    setTodoLists(modified);
  }

  function onTextChange(event: React.ChangeEvent<HTMLInputElement>) {
    let elementTree = event.target.id.split(":");
    let modified = [...todoLists];
    if (elementTree.length === 3) {
      // Changing the description of a list item
      modified[parseInt(elementTree[0])].todos[parseInt(elementTree[1])].description = event.target.value;
    } else {
      // Changing the name of a list
      modified[parseInt(elementTree[0])].name = event.target.value;
    }
  }

  async function onTextFocusOut(event: React.FocusEvent<HTMLInputElement, Element>) {
    let elementTree = event.target.id.split(":");
    try {
      const token = isAuthenticated
        ? await getAccessTokenSilently({
            authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
          })
        : undefined;
      await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])], token);
    } catch (apiError: any) {
      logAndSetError(apiError);
      return;
    }
  }

  async function onTodoItemKeyDown(event: React.KeyboardEvent<HTMLInputElement>) {
    let elementTree = event.currentTarget.id.split(":");
    if (event.key === "Enter") {
      await onKeyEnter();
    } else if (
      event.key === "Backspace" &&
      event.currentTarget.value === "" &&
      todoLists[parseInt(elementTree[0])].todos.length !== 1
    ) {
      await onKeyBackspace();
    } else if (
      event.key === "Delete" &&
      event.currentTarget.value === "" &&
      todoLists[parseInt(elementTree[0])].todos.length !== 1
    ) {
      await onKeyDelete();
    }

    async function onKeyEnter() {
      let modified = [...todoLists];
      var newtodo: Todo = { description: "", isDone: false };
      modified[parseInt(elementTree[0])].todos.splice(parseInt(elementTree[1]) + 1, 0, newtodo);
      try {
        const token = isAuthenticated
          ? await getAccessTokenSilently({
              authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
            })
          : undefined;
        await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])], token);
      } catch (apiError: any) {
        logAndSetError(apiError);
        return;
      }
      setTodoLists(modified);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1]) + 1}:text`;
        document.getElementById(itemToFocus)!.focus();
      }, 5);
    }

    async function onKeyBackspace() {
      let modified = [...todoLists];
      modified[parseInt(elementTree[0])].todos.splice(parseInt(elementTree[1]), 1);
      try {
        const token = isAuthenticated
          ? await getAccessTokenSilently({
              authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
            })
          : undefined;
        await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])], token);
      } catch (apiError: any) {
        logAndSetError(apiError);
        return;
      }
      setTodoLists(modified);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1]) - 1}:text`;
        document.getElementById(itemToFocus)?.focus();
      }, 5);
    }

    async function onKeyDelete() {
      let modified = [...todoLists];
      modified[parseInt(elementTree[0])].todos.splice(parseInt(elementTree[1]), 1);
      try {
        const token = isAuthenticated
          ? await getAccessTokenSilently({
              authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
            })
          : undefined;
        await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])], token);
      } catch (apiError: any) {
        logAndSetError(apiError);
        return;
      }
      setTodoLists(modified);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1])}:text`;
        document.getElementById(itemToFocus)?.focus();
      }, 5);
    }
  }

  return (
    <div className="App">
      {isAuthenticated ? (
        <>
          <Profile />
          {apiError === "" ? (
            <>
              <h1>Your Todos:</h1>
              <ul>
                {todoLists.map((todoList, listIndex) => (
                  <li key={uuidv4()} id={todoList.id!.toString()}>
                    <div className="todoLists">
                      <input
                        className="todoListName"
                        type="text"
                        size={54}
                        id={`${listIndex}:text`}
                        defaultValue={todoList.name}
                        onChange={onTextChange}
                        onBlur={onTextFocusOut}
                      />
                      <button onClick={onDeleteListClick} title="Delete this list">
                        -
                      </button>
                    </div>
                    <ul>
                      {todoList.todos.map((todoItem, itemIndex) => (
                        <li key={uuidv4()}>
                          <div className="todoItems">
                            <input
                              type="checkbox"
                              id={`${listIndex}:${itemIndex}:checkbox`}
                              checked={todoItem.isDone}
                              onChange={onItemDoneChange}
                              title="Mark as done"
                            />
                            <input
                              style={{ textDecorationLine: todoItem.isDone ? "line-through" : "" }}
                              disabled={todoItem.isDone}
                              type="text"
                              size={50}
                              id={`${listIndex}:${itemIndex}:text`}
                              defaultValue={todoItem.description}
                              onKeyDown={onTodoItemKeyDown}
                              onChange={onTextChange}
                              onBlur={onTextFocusOut}
                            />
                          </div>
                        </li>
                      ))}
                    </ul>
                  </li>
                ))}
                <br />
                <button id="addNewListBtn" onClick={onAddNewListClick} title="Add new list">
                  Add New List
                </button>
              </ul>
            </>
          ) : (
            <>
              <h1 style={{ color: "red" }}>Error occurred!</h1>
              <h2>{apiError}</h2>
              <h3>
                Please verify that TodoApi is running and reachable in <code>{todoApi.apiBaseUrl}</code>
              </h3>
            </>
          )}
        </>
      ) : (
        <div className="action-card">
          <p className="action-text">Get started by signing in to your account</p>
          <LoginButton />
        </div>
      )}
    </div>
  );
}

export default App;
