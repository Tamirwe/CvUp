import { AuthCardWrapper } from "./AuthCardWrapper";
import { AuthWrapper } from "./AuthWrapper";
import { PasswordResetWrapper } from "./PasswordResetWrapper";

export const PasswordReset = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <PasswordResetWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
