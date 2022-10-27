import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { useState } from "react";
import { FiCheckCircle } from "react-icons/fi";
import { Link } from "react-router-dom";
import { RegisterForm } from "./RegisterFrom";

export const RegisterWrapper = () => {
  const theme = useTheme();
  const [isVerificationEmailSent, setIsVerificationEmailSent] = useState(false);
  const [email, setEmail] = useState("");

  return (
    <>
      {!isVerificationEmailSent ? (
        <Grid container>
          <Grid item xs={12} textAlign="center" mb={4}>
            <FiCheckCircle fontSize={55} color="green" />
          </Grid>
          <Grid item xs={12} textAlign="center" mb={4}>
            <Typography variant="h5">Thanks for signing up!</Typography>
          </Grid>
          <Grid item xs={12} mb={2} textAlign="center">
            <Typography variant="subtitle2">
              An email has been sent to {email}
            </Typography>
          </Grid>
          <Grid item xs={12} textAlign="center" mb={2}>
            <Typography variant="body2">
              with instruction for verifing your account.
            </Typography>
          </Grid>
          <Grid item xs={12} textAlign="center" mb={2}>
            <Typography variant="body2">
              If you have not recived the verification email, please check your
              "Spam" folder.
            </Typography>
          </Grid>
          <Grid item xs={12} mb={2} textAlign="center">
            <hr />
            <Typography variant="subtitle2" component="p">
              Need help? Contact support.
            </Typography>
          </Grid>
        </Grid>
      ) : (
        <Grid container>
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
                Sign up
              </Typography>
              <Typography component={Link} to="/login" variant="subtitle2">
                Already have an account?
              </Typography>
            </Stack>
          </Grid>

          <Grid item xs={12}>
            <Grid item sx={{ mt: 2 }}>
              <Typography variant="subtitle1" fontSize="16px">
                Enter your credentials to continue
              </Typography>
            </Grid>
          </Grid>
          <Grid item xs={12}>
            <RegisterForm
              registerFormComplete={(email) => {
                setEmail(email);
                setIsVerificationEmailSent(true);
              }}
            />
          </Grid>
        </Grid>
      )}
    </>
  );
};
