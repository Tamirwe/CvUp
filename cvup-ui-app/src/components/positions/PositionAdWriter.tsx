import { Button, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";

export const PositionAdWriter = observer(() => {
  const { positionsStore } = useStore();
  const position = positionsStore.editPosition;
  const [positionAd, setPositionAd] = useState(position?.positionAd ?? "");

  useEffect(() => {
    fetchAd();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const fetchAd = async () => {
    if (position) {
      const result = await positionsStore.positionAdAiWriter(position);
      if (result) {
        setPositionAd(result);
      }
    }
  };

  return (
    <>
      <TextField
        sx={{ direction: "rtl" }}
        fullWidth
        multiline
        rows={20}
        margin="normal"
        label="Position Ad"
        variant="outlined"
        value={positionAd}
        onChange={(e) => setPositionAd(e.target.value)}
      />
      <Button variant="outlined" color="primary" onClick={fetchAd}>
        Update Ad
      </Button>
    </>
  );
});
