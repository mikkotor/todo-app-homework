export type Todo = {
  description: string;
  isDone: boolean;
};

export interface TodoList {
  id: number;
  name: string;
  todos: Todo[];
};
