import { AuthCardWrapper } from "./AuthCardWrapper";
import { AuthWrapper } from "./AuthWrapper";
import { ForgotPasswordWrapper } from "./ForgotPasswordWrapper";

export const ForgotPassword = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <ForgotPasswordWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
