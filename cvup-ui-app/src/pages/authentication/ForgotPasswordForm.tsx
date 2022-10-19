import { Box, Button, TextField } from "@mui/material";

interface props {}

export const ForgotPasswordForm = ({}: props) => {
  const handleUserNameChange = () => {};

  return (
    <form noValidate>
      <TextField
        fullWidth
        required
        margin="normal"
        type="text"
        id="userNameTxt"
        label="Email Address / Username"
        variant="outlined"
        onChange={handleUserNameChange}
      />
      <Box sx={{ mt: 4 }}>
        <Button
          // disableElevation
          // disabled={isSubmitting}
          fullWidth
          size="large"
          type="submit"
          variant="contained"
          color="secondary"
        >
          Reset Password
        </Button>
      </Box>
    </form>
  );
};
