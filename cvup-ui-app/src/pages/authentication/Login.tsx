import { AuthCardWrapper } from "./AuthCardWrapper";
import { AuthWrapper } from "./AuthWrapper";
import { LoginWrapper } from "./LoginWrapper";

export const Login = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <LoginWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
