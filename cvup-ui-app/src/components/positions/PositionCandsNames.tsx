import { Button, TextField } from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { copyToClipBoard } from "../../utils/GeneralUtils";

export const PositionCandsNames = observer(() => {
  const { candsStore, generalStore, positionsStore } = useStore();
  const positionId = positionsStore.editPosition?.id;
  const [candsNames, setCandsNames] = useState("");

  useEffect(() => {
    fetchCandsNames();
  }, [positionId]); // eslint-disable-line react-hooks/exhaustive-deps

  const fetchCandsNames = async () => {
    if (positionId) {
      const cands = await candsStore.fetchPositionCands(positionId);
      const names = cands
        .map(
          (cand) =>
            `${cand.firstName?.trim() || ""} ${cand.lastName?.trim() || ""}`.trim() ||
            cand.email ||
            ""
        )
        .filter((name) => name);

      setCandsNames(names.join("\n"));
    }
  };

  const copyCandsNames = async () => {
    const isCopied = await copyToClipBoard(candsNames);

    if (isCopied) {
      generalStore.alertSnackbar("success", "Copied");
    } else {
      generalStore.alertSnackbar("error", "Failed to copy");
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
        label="Candidates"
        variant="outlined"
        value={candsNames}
        onChange={(e) => setCandsNames(e.target.value)}
      />
      <Button variant="outlined" color="primary" onClick={copyCandsNames}>
        Copy candidates list
      </Button>
    </>
  );
});
