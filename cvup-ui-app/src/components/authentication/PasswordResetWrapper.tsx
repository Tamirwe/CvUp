import { Grid, Stack, Typography, useTheme } from "@mui/material";
import { PasswordResetForm } from "./PasswordResetForm";

export const PasswordResetWrapper = () => {
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
            Reset Password
          </Typography>
        </Stack>
      </Grid>
      <Grid item xs={12} sx={{ mt: 4 }}>
        <PasswordResetForm />
      </Grid>
    </Grid>
  );
};
