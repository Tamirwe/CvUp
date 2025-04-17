import { Button, FormHelperText, Grid, Stack, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { isMobile } from "react-device-detect";
import useDebounce from "../../Hooks/useDebounce";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const ReviewCandForm = observer(({ onSaved, onCancel }: IProps) => {
  const { candsStore, generalStore } = useStore();
  const [review, setReview] = useState("");
  const [submitError, setSubmitError] = useState("");
  const debouncedValue = useDebounce<string>(review, 2000);
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    if (isLoaded) {
      candsStore.saveReviewToLocalStorage(review);
    }
  }, [debouncedValue]);

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

  useEffect(() => {
    if (!isMobile && generalStore.showInterviewFullDialog) {
      setReview(candsStore.candReviewSync);
    }
  }, [generalStore.showInterviewFullDialog]);

  return (
    <>
      <form
        noValidate
        spellCheck="false"
        style={{ height: "100%", width: "100%" }}
      >
        <Grid container sx={{ direction: "rtl", height: "100%" }} gap={0}>
          <Grid item xs={12} lg={12} style={{ minHeight: "90%" }}>
            <TextField
              inputProps={{
                style: {
                  height: "94%",
                  overflow: "auto",
                },
              }}
              sx={{
                direction: "rtl",
                height: "100%",
                "& .MuiInputBase-root": { height: "100%" },
                "& .MuiInputBase-input": { height: "100%" },
              }}
              fullWidth
              multiline
              // rows={isMobile ? 18 : 27}
              // rows={100}
              margin="normal"
              type="text"
              id="description"
              label="Description"
              variant="outlined"
              onChange={(e) => {
                setReview(e.target.value);
                !isLoaded && setIsLoaded(true);
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
            <Grid container justifyContent="end">
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
