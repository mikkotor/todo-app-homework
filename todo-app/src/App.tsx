import React, { useState, useEffect } from "react";
// import logo from "./logo.svg";
import "./App.css";
import { TodoList, Todo } from "./todoTypes";
import { TodoApi } from "./todoApi";
import { v4 as uuidv4 } from "uuid";

function App() {
  const todoApi = new TodoApi();
  const [todoLists, setTodoLists] = useState<TodoList[]>([]);

  useEffect(() => {
    todoApi.getTodoListsAsync().then((data) => setTodoLists(data));
    // eslint-disable-next-line
  }, []);

  async function onAddNewListClick(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    let newTodoList = { name: "New list", id: 0, todos: [{ description: "First item", isDone: false }] };
    newTodoList.id = await todoApi.upsertTodoListAsync(newTodoList);
    let modified = [...todoLists];
    modified.push(newTodoList);
    setTodoLists(modified);
  }

  async function onDeleteListClick(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
    console.log(event.currentTarget.parentElement?.parentElement);
    if (event.currentTarget.parentElement?.parentElement?.id === undefined) return;
    let databaseId: number = parseInt(event.currentTarget.parentElement.parentElement.id);
    let modified = [...todoLists];
    var found = modified.findIndex((i) => i.id === databaseId);
    if (found === -1) return;
    modified.splice(found, 1);
    setTodoLists(modified);
    if (databaseId !== 0) await todoApi.deleteTodoListAsync(databaseId);
  }

  async function onItemDoneChange(event: React.ChangeEvent<HTMLInputElement>) {
    let elementTree = event.target.id.split(":");
    let modified = [...todoLists];
    modified[parseInt(elementTree[0])].todos[parseInt(elementTree[1])].isDone = event.target.checked;
    setTodoLists(modified);
    await todoApi.upsertTodoListAsync(modified[parseInt(elementTree[0])]);
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
    await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])]);
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
      setTodoLists(modified);
      await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])]);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1]) + 1}:text`;
        document.getElementById(itemToFocus)?.focus();
      }, 0);
    }

    async function onKeyBackspace() {
      let modified = [...todoLists];
      modified[parseInt(elementTree[0])].todos.splice(parseInt(elementTree[1]), 1);
      setTodoLists(modified);
      await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])]);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1]) - 1}:text`;
        document.getElementById(itemToFocus)?.focus();
      }, 0);
    }

    async function onKeyDelete() {
      let modified = [...todoLists];
      modified[parseInt(elementTree[0])].todos.splice(parseInt(elementTree[1]), 1);
      setTodoLists(modified);
      await todoApi.upsertTodoListAsync(todoLists[parseInt(elementTree[0])]);
      setTimeout(() => {
        let itemToFocus = `${elementTree[0]}:${parseInt(elementTree[1])}:text`;
        document.getElementById(itemToFocus)?.focus();
      }, 0);
    }
  }

  return (
    <div className="App">
      <h1>Your Todos:</h1>
      <ul>
        {todoLists.map((todoList, listIndex) => (
          <li key={uuidv4()} id={todoList.id.toString()}>
            <div className="todoLists">
              <input
                className="todoListName"
                type="text"
                size={54}
                id={`${listIndex.toString()}:text`}
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
                      id={`${listIndex.toString()}:${itemIndex.toString()}:checkbox`}
                      checked={todoItem.isDone}
                      onChange={onItemDoneChange}
                    />
                    <input
                      style={{ textDecorationLine: todoItem.isDone ? "line-through" : "" }}
                      type="text"
                      size={50}
                      id={`${listIndex.toString()}:${itemIndex.toString()}:text`}
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
    </div>
  );
}

export default App;
