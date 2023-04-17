describe("When GET fails", () => {
  it("renders the error view", () => {
    cy.intercept("GET", "/Todo", {
      statusCode: 404,
    });

    cy.visit("/");
    cy.get("h1").contains("Error occurred!");
  });
});

describe("When POST fails", () => {
  it("renders the error view", () => {
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

    cy.intercept("POST", "/Todo", {
      statusCode: 500,
    });

    cy.visit("/");
    cy.get("#addNewListBtn").click();
    cy.get("h1").contains("Error occurred!");
  });
});

describe("When PATCH fails", () => {
  it("renders the error view", () => {
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

    cy.intercept("PATCH", "/Todo", {
      statusCode: 500,
    });

    cy.visit("/");
    cy.get(".todoLists:first input:text").type(" now edited");
    cy.get(".todoLists:first input:text").should("have.value", "My very first todo list now edited")
    cy.get("h1").click();
    cy.get("h1").contains("Error occurred!");
  });
});

describe("When DELETE fails", () => {
  it("renders the error view", () => {
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

    cy.intercept("DELETE", "/Todo?id=1", {
      statusCode: 500,
    });

    cy.visit("/");
    cy.get(".todoLists:first").find("button").click();
    cy.get("h1").contains("Error occurred!");
  });
});
