import { AuthCardWrapper } from "../../components/authentication/AuthCardWrapper";
import { AuthWrapper } from "../../components/authentication/AuthWrapper";
import { LoginWrapper } from "../../components/authentication/LoginWrapper";

export const Login = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <LoginWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
