import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { isMobile } from "react-device-detect";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const ReviewCandForm = observer(({ onSaved, onCancel }: IProps) => {
  const { candsStore } = useStore();
  const [review, setReview] = useState("");
  const [submitError, setSubmitError] = useState("");

  useEffect(() => {
    setReview(candsStore.candDisplay?.review || "");
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleSubmit = async () => {
    const res = await candsStore.saveCandReview(review);

    if (res?.isSuccess) {
      onSaved();
    } else {
      setSubmitError(res?.errorData?.message);
    }
  };

  return (
    <>
      <form noValidate spellCheck="false">
        <Grid container sx={{ direction: "rtl" }} gap={0}>
          <Grid item xs={12} lg={12}>
            <TextField
              sx={{
                direction: "rtl",
                "& .MuiInputBase-input": { maxHeight: "calc(100% - 21rem)" },
              }}
              fullWidth
              multiline
              rows={isMobile ? 18 : 27}
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

          {submitError && (
            <Grid item xs={12}>
              <div style={{ direction: "ltr" }}>
                <FormHelperText error>{submitError}</FormHelperText>
              </div>
            </Grid>
          )}

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
