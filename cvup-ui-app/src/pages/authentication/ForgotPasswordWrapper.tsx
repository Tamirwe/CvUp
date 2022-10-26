import { Grid, Typography, useTheme } from "@mui/material";
import { useState } from "react";
import { Link } from "react-router-dom";
import { ForgotPasswordForm } from "./ForgotPasswordForm";

export const ForgotPasswordWrapper = () => {
  const theme = useTheme();
  const [isEmailResetSent, setIsEmailResetSent] = useState(false);
  const [email, setEmail] = useState("");

  return (
    <>
      {!isEmailResetSent ? (
        <Grid container>
          <Grid item xs={12}>
            <Typography
              color={theme.palette.secondary.main}
              gutterBottom
              variant="h5"
            >
              Reset Your Password
            </Typography>
          </Grid>
          <Grid item xs={12} sx={{ mt: 4 }}>
            <ForgotPasswordForm
              resetPasswordSent={(email) => {
                setEmail(email);
                setIsEmailResetSent(true);
              }}
            />
          </Grid>
          <Grid
            item
            xs={12}
            sx={{ mt: 4 }}
            justifyContent="center"
            textAlign="center"
          >
            <Typography component={Link} to="/login" variant="subtitle2">
              Return to Login
            </Typography>
          </Grid>
        </Grid>
      ) : (
        <Grid container>
          <Typography variant="subtitle2">Check your email</Typography>
          <Typography variant="subtitle2">
            We've sent password reset instructions to:
          </Typography>
          <Typography variant="subtitle2">{email}</Typography>
          <Typography variant="subtitle2">
            If it doesn't arrive soon, check your spam folder or
          </Typography>
          <Typography variant="subtitle2">send the email again</Typography>
          <Typography variant="subtitle2">
            Need help? Contact support.
          </Typography>
        </Grid>
      )}
    </>
  );
};
