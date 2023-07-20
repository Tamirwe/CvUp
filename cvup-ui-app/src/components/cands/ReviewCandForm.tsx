import { Button, Grid, Stack, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const ReviewCandForm = observer(({ onSaved, onCancel }: IProps) => {
  const { candsStore } = useStore();
  const [review, setReview] = useState("");

  useEffect(() => {
    setReview(candsStore.candDisplay?.review || "");
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleSubmit = async () => {
    await candsStore.saveCandReview(review);
    onSaved();
  };

  return (
    <>
      <form noValidate spellCheck="false">
        <Grid container>
          <Grid item xs={12} lg={12} sx={{ direction: "rtl" }}>
            <TextField
              sx={{
                direction: "rtl",
              }}
              fullWidth
              multiline
              rows={27}
              margin="normal"
              type="text"
              id="description"
              label="Description"
              variant="outlined"
              onChange={(e) => {
                setReview(e.target.value);
              }}
              value={review}
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
