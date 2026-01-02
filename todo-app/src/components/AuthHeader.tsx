import React from "react";
import { useAuth0 } from "@auth0/auth0-react";

export default function AuthHeader() {
  const { loginWithRedirect, logout, isAuthenticated, user, isLoading } = useAuth0();

  if (isLoading) return <header style={{ padding: 8 }}>Authenticating...</header>;

  return (
    <header style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: 8 }}>
      <div>
        <strong>Todo App</strong>
      </div>
      <div>
        {isAuthenticated ? (
          <>
            <span style={{ marginRight: 12 }}>{user?.name ?? user?.email}</span>
            <button onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}>Log out</button>
          </>
        ) : (
          <button onClick={() => void loginWithRedirect()}>Log in</button>
        )}
      </div>
    </header>
  );
}
