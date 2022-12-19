import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import { LOGIN_TYPE } from "../../constants/AuthConsts";
import { LoginForm } from "./LoginForm";

interface IProps {
  loginType: string;
}

export const LoginWrapper = ({ loginType }: IProps) => {
  const ref = useRef<HTMLDivElement>(null);
  const theme = useTheme();

  useEffect(() => {
    ref.current?.click();
  });

  const title = () => {
    switch (loginType) {
      case LOGIN_TYPE.REGULAR_LOGIN:
        return "Login";
      case LOGIN_TYPE.COMPLETE_REGISTRATION_LOGIN:
        return "Complete Registration";
      case LOGIN_TYPE.PASSWORD_RESET_LOGIN:
        return "Reset Password";
    }
  };

  return (
    <Grid container ref={ref}>
      <Grid item xs={12}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          spacing={1}
        >
          <Typography
            color={theme.palette.secondary.main}
            gutterBottom
            variant="h5"
          >
            {title()}
          </Typography>
          {loginType === "login" && (
            <Typography component={Link} to="/register" variant="subtitle2">
              Don&apos;t have an account?
            </Typography>
          )}
        </Stack>
      </Grid>
      <Grid item xs={12} sx={{ mt: 4 }}>
        <LoginForm loginType={loginType} />
      </Grid>
    </Grid>
  );
};
