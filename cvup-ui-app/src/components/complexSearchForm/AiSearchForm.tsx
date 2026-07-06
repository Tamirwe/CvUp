import { Box, Button, Divider, Stack, TextField } from "@mui/material";
import { runInAction } from "mobx";
import { observer } from "mobx-react";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onClose: () => void;
}

export const AiSearchForm = observer(({ onClose }: IProps) => {
  const { candsStore } = useStore();

  const handleSearch = () => {
    if (candsStore.aiSearchText.trim()) {
      candsStore.AiSearchCands({ value: candsStore.aiSearchText.trim(), exact: false, luceneFilter: candsStore.aiLuceneFilter.trim() || undefined });
      onClose();
    }
  };

  return (
    <Box sx={{ direction: "rtl" }}>
      <TextField
        fullWidth
        size="small"
        label="Pre-filter by keywords (Lucene exact search)"
        placeholder="e.g. csharp, fintech"
        value={candsStore.aiLuceneFilter}
        onChange={(e) => runInAction(() => { candsStore.aiLuceneFilter = e.target.value; })}
        sx={{ mb: 2.5, direction: "rtl" }}
      />
      <Divider sx={{ mb: 2 }} />
      <TextField
        fullWidth
        multiline
        rows={4}
        label="Describe the candidate you are looking for"
        placeholder="e.g. Senior C# developer with fintech experience and strong SQL skills..."
        value={candsStore.aiSearchText}
        onChange={(e) => runInAction(() => { candsStore.aiSearchText = e.target.value; })}
        sx={{ mb: 2.5, direction: "rtl" }}
        autoFocus
      />
      <Stack direction="row" justifyContent="flex-end">
        <Button
          variant="contained"
          color="primary"
          onClick={handleSearch}
          disabled={!candsStore.aiSearchText.trim()}
          sx={{ px: 4 }}
        >
          Search
        </Button>
      </Stack>
    </Box>
  );
});