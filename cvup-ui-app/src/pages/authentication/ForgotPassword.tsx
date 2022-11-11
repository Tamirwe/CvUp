import { AuthCardWrapper } from "../../components/authentication/AuthCardWrapper";
import { AuthWrapper } from "../../components/authentication/AuthWrapper";
import { ForgotPasswordWrapper } from "../../components/authentication/ForgotPasswordWrapper";

export const ForgotPassword = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <ForgotPasswordWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
