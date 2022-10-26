import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { useState } from "react";
import { Link } from "react-router-dom";
import { ForgotPasswordForm } from "./ForgotPasswordForm";
import { FiCheckCircle } from "react-icons/fi";

export const ForgotPasswordWrapper = () => {
  const theme = useTheme();
  const [isEmailResetSent, setIsEmailResetSent] = useState(false);
  const [email, setEmail] = useState("");

  return (
    <>
      {isEmailResetSent ? (
        <Grid container>
          <Grid item xs={12} textAlign="center" mb={4}>
            <FiCheckCircle fontSize={55} color="green" />
          </Grid>
          <Grid item xs={12} textAlign="center" mb={4}>
            <Typography variant="h5">Password Reset Email Sent</Typography>
          </Grid>
          <Grid item xs={12} mb={2} textAlign="center">
            <Typography variant="subtitle2">
              An email has been sent to your email address,
            </Typography>
          </Grid>
          <Grid item xs={12} textAlign="center">
            <Typography variant="subtitle2">{email}</Typography>
          </Grid>
          <Grid item xs={12} textAlign="center" mb={2}>
            <Typography variant="body2">
              Follow the directions in the email to reset your password.
            </Typography>
          </Grid>
          <Grid item xs={12} mb={2} textAlign="center">
            <Typography variant="subtitle2" component="p">
              Need help? Contact support.
            </Typography>
          </Grid>
        </Grid>
      ) : (
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
      )}
    </>
  );
};
