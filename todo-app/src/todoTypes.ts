export type Todo = {
  description: string;
  isDone: boolean;
};

export type TodoList = {
  id?: string;
  name: string;
  todos: Todo[];
};
