import { useAuth0, AuthorizationParams } from "@auth0/auth0-react";

const LoginButton = () => {
  const { loginWithRedirect } = useAuth0();
  return (
    <button
      onClick={() =>
        loginWithRedirect({
          authorizationParams: { audience: import.meta.env.VITE_AUTH0_AUDIENCE as string } as AuthorizationParams,
        })
      }
      className="button login"
    >
      Log In
    </button>
  );
};

export default LoginButton;
