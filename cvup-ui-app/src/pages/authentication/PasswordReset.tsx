import { AuthCardWrapper } from "../../components/authentication/AuthCardWrapper";
import { AuthWrapper } from "../../components/authentication/AuthWrapper";
import { PasswordResetWrapper } from "../../components/authentication/PasswordResetWrapper";

export const PasswordReset = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <PasswordResetWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
