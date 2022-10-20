import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { Link } from "react-router-dom";
import { RegisterForm } from "./RegisterFrom";

export const RegisterWrapper = () => {
  const theme = useTheme();

  return (
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
        <RegisterForm />
      </Grid>
    </Grid>
  );
};
