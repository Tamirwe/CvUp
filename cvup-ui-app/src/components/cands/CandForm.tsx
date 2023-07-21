import { Button, Grid, Stack, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const CandForm = observer(({ onSaved, onCancel }: IProps) => {
  const { candsStore } = useStore();
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");

  useEffect(() => {
    setFirstName(candsStore.candDisplay?.firstName || "");
    setLastName(candsStore.candDisplay?.lastName || "");
    setEmail(candsStore.candDisplay?.email || "");
    setPhone(candsStore.candDisplay?.phone || "");
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleSubmit = async () => {
    await candsStore.saveCandDetails(firstName, lastName, email, phone);
    onSaved();
  };

  return (
    <>
      <form noValidate spellCheck="false">
        <Grid container sx={{ direction: "rtl" }} gap={0}>
          <Grid item xs={6} lg={6}>
            <TextField
              sx={{
                direction: "rtl",
              }}
              fullWidth
              margin="dense"
              type="text"
              id="firstName"
              label="First Name"
              variant="outlined"
              onChange={(e) => {
                setFirstName(e.target.value);
              }}
              value={firstName}
            />
          </Grid>
          <Grid item xs={6} lg={6}>
            <TextField
              sx={{
                direction: "rtl",
              }}
              fullWidth
              margin="dense"
              type="text"
              id="lastName"
              label="Last Name"
              variant="outlined"
              onChange={(e) => {
                setLastName(e.target.value);
              }}
              value={lastName}
            />
          </Grid>
          <Grid item xs={12} lg={6}>
            <TextField
              sx={{
                direction: "rtl",
              }}
              fullWidth
              margin="dense"
              type="text"
              id="email"
              label="Email"
              variant="outlined"
              onChange={(e) => {
                setEmail(e.target.value);
              }}
              value={email}
            />
          </Grid>
          <Grid item xs={12} lg={6}>
            <TextField
              sx={{
                direction: "rtl",
              }}
              fullWidth
              margin="dense"
              type="text"
              id="phone"
              label="Phone"
              variant="outlined"
              onChange={(e) => {
                setPhone(e.target.value);
              }}
              value={phone}
            />
          </Grid>

          <Grid item xs={12} mt={4}>
            <Grid container justifyContent="space-between">
              <Grid item>
                <Stack direction="row" alignItems="center" gap={1}>
                  <Button
                    fullWidth
                    color="secondary"
                    onClick={() => onCancel()}
                  >
                    Cancel
                  </Button>

                  <Button fullWidth color="secondary" onClick={handleSubmit}>
                    Save
                  </Button>
                </Stack>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </form>
    </>
  );
});
