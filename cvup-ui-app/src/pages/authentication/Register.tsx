import { AuthCardWrapper } from "./AuthCardWrapper";
import { AuthWrapper } from "./AuthWrapper";
import { RegisterWrapper } from "./RegisterWrapper";

export const Register = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <RegisterWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
