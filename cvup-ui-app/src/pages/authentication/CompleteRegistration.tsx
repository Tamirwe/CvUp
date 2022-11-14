import { AuthCardWrapper } from "../../components/authentication/AuthCardWrapper";
import { AuthWrapper } from "../../components/authentication/AuthWrapper";
import { LoginWrapper } from "../../components/authentication/LoginWrapper";
import { LOGIN_TYPE } from "../../constants/AuthConsts";

export const CompleteRegistration = () => {
  return (
    <AuthWrapper>
      <AuthCardWrapper>
        <LoginWrapper loginType={LOGIN_TYPE.COMPLETE_REGISTRATION_LOGIN} />
      </AuthCardWrapper>
    </AuthWrapper>
  );
};
