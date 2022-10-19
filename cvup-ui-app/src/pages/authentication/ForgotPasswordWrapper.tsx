import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { Link } from "react-router-dom";
import { ForgotPasswordForm } from "./ForgotPasswordForm";

interface props {}

export const ForgotPasswordWrapper = ({}: props) => {
  const theme = useTheme();

  return (
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
        <ForgotPasswordForm />
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
  );
};
