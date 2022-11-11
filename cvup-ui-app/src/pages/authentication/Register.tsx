import { AuthCardWrapper } from "../../components/authentication/AuthCardWrapper";
import { AuthWrapper } from "../../components/authentication/AuthWrapper";
import { RegisterWrapper } from "../../components/authentication/RegisterWrapper";

export const Register = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <RegisterWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
