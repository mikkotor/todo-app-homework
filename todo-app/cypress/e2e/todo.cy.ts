describe("When no lists yet", () => {
  it("page has correct elements", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      []
    );

    cy.visit("/");
    cy.get("h1").contains("Your Todos:");
    cy.get("#addNewListBtn").should("exist").and("contain.text", "Add New List");
    cy.get(".todoListName").should("not.exist");
  });

  it("new lists can be added", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      []
    );
    cy.intercept(
      {
        method: "POST",
        url: "/Todo",
      },
      "1"
    );

    cy.visit("/");
    cy.get("#addNewListBtn").click();
    cy.get("li").eq(0).should("exist").and("have.id", "1");
  });
});

describe("When lists with items", () => {
  it("lists are rendered", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      [
        {
          id: 1,
          name: "My very first todo list",
          todos: [
            {
              description: "Write code",
              isDone: true,
            },
            {
              description: "Write tests",
              isDone: true,
            },
            {
              description: "Get a job",
              isDone: false,
            },
          ],
        },
      ]
    );

    cy.visit("/");
    cy.get(".todoListName:first").should("have.value", "My very first todo list");

    cy.get("ul:first ul:first li input:text").eq(0).should("have.value", "Write code");
    cy.get("ul:first ul:first li input:checkbox").eq(0).should("have.attr", "checked");

    cy.get("ul:first ul:first li input:text").eq(1).should("have.value", "Write tests");
    cy.get("ul:first ul:first li input:checkbox").eq(1).should("have.attr", "checked");

    cy.get("ul:first ul:first li input:text").eq(2).should("have.value", "Get a job");
    cy.get("ul:first ul:first li input:checkbox").eq(2).should("not.have.attr", "checked");
  });

  it("list names can be changed", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      [
        {
          id: 1,
          name: "My very first todo list",
          todos: [
            {
              description: "Write code",
              isDone: true,
            },
            {
              description: "Write tests",
              isDone: true,
            },
            {
              description: "Get a job",
              isDone: false,
            },
          ],
        },
      ]
    );
    cy.intercept(
      {
        method: "PATCH",
        url: "/Todo",
      },
      { statusCode: 200 }
    );

    cy.visit("/");
    cy.get(".todoLists:first input:text").type(" now edited");
    cy.get(".todoLists:first input:text").should("have.value", "My very first todo list now edited")
    cy.get("h1").click();
  });

  it("item names can be changed", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      [
        {
          id: 1,
          name: "My very first todo list",
          todos: [
            {
              description: "Write code",
              isDone: true,
            },
            {
              description: "Write tests",
              isDone: true,
            },
            {
              description: "Get a job",
              isDone: false,
            },
          ],
        },
      ]
    );
    cy.intercept(
      {
        method: "PATCH",
        url: "/Todo",
      },
      { statusCode: 200 }
    );

    cy.visit("/");
    cy.get("ul:first ul:first li input:text").eq(2).type(", wouldn't that be nice");
    cy.get("ul:first ul:first li input:text").eq(2).should("have.value", "Get a job, wouldn't that be nice");
    cy.get("h1").click();
  });

  it("items can be deleted", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      [
        {
          id: 1,
          name: "My very first todo list",
          todos: [
            {
              description: "Write code",
              isDone: true,
            },
            {
              description: "Write tests",
              isDone: true,
            },
            {
              description: "X",
              isDone: false,
            },
          ],
        },
      ]
    );
    cy.intercept(
      {
        method: "PATCH",
        url: "/Todo",
      },
      { statusCode: 200 }
    );

    cy.visit("/");

    cy.get("ul:first ul:first li input:text").eq(2).type("{backspace}{backspace}");
    cy.get("ul:first ul:first li input:text").eq(2).should("not.exist");
  });

  it("lists can be deleted", () => {
    cy.intercept(
      {
        method: "GET",
        url: "/Todo",
      },
      [
        {
          id: 1,
          name: "My very first todo list",
          todos: [
            {
              description: "Write code",
              isDone: true,
            },
            {
              description: "Write tests",
              isDone: true,
            },
            {
              description: "X",
              isDone: false,
            },
          ],
        },
        {
          id: 2,
          name: "My second todo list",
          todos: [
            {
              description: "Foo",
              isDone: true,
            },
            {
              description: "Bar",
              isDone: true,
            },
          ],
        },
      ]
    );
    cy.intercept(
      {
        method: "DELETE",
        url: "/Todo?id=2",
      },
      { statusCode: 200 }
    );

    cy.visit("/");

    cy.get(".todoLists").eq(1).find("button").click();
    cy.get(".todoLists").eq(1).should("not.exist");
  });
});
