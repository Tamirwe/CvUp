import {
  Grid,
  Stack,
  Typography,
  useMediaQuery,
  useTheme,
  Divider,
} from "@mui/material";
import { Link } from "react-router-dom";
import { RegisterForm } from "./RegisterFrom";

interface props {}

export const RegisterWrapper = ({}: props) => {
  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));

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
      <Grid>
        <Divider sx={{ mt: 2 }} />
      </Grid>
      <Grid item xs={12} sx={{ mt: 2 }}>
        <Grid item container direction="column" alignItems="center" xs={12}>
          <Typography component={Link} to="/login" variant="subtitle2">
            Already have an account?
          </Typography>
        </Grid>
      </Grid>
    </Grid>
  );
};
