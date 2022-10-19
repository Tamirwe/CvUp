import { AuthCardWrapper } from "./AuthCardWrapper";
import { AuthWrapper } from "./AuthWrapper";
import { RegisterWrapper } from "./RegisterWrapper";

interface props {}

export const Register = ({}: props) => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <RegisterWrapper />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};