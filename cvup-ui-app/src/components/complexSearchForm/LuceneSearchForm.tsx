import {
  Box,
  Button,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { runInAction } from "mobx";
import { observer } from "mobx-react";
import { IComplexSearchTerm, TermOccur } from "../../models/GeneralModels";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onClose: () => void;
}

export const LuceneSearchForm = observer(({ onClose }: IProps) => {
  const { candsStore } = useStore();

  const handleClear = () => {
    runInAction(() => {
      candsStore.luceneFirstMust = "";
      candsStore.luceneFirstShould = "";
      candsStore.luceneWithinMust = "";
      candsStore.luceneWithinShould = "";
    });
  };

  const handleSearch = () => {
    const firstSearch: IComplexSearchTerm[] = [
      ...parseTerms(candsStore.luceneFirstMust, "Must"),
      ...parseTerms(candsStore.luceneFirstShould, "Should"),
    ];

    const withinTerms: IComplexSearchTerm[] = [
      ...parseTerms(candsStore.luceneWithinMust, "Must"),
      ...parseTerms(candsStore.luceneWithinShould, "Should"),
    ];

    const searchWithin = withinTerms.length > 0 ? withinTerms : undefined;

    candsStore.complexSearchCands(firstSearch, searchWithin);
    onClose();
  };

  return (
    <Box sx={{ direction: "rtl" }}>
      <SectionLabel label="Search" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. React, TypeScript, Node"
          value={candsStore.luceneFirstMust}
          onChange={(e) => runInAction(() => { candsStore.luceneFirstMust = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
        <TextField
          fullWidth
          size="small"
          label="Should have — separate by comma"
          placeholder="e.g. AWS, Docker"
          value={candsStore.luceneFirstShould}
          onChange={(e) => runInAction(() => { candsStore.luceneFirstShould = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
      </Stack>

      <SectionLabel label="Search within results" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. clean rooms, automation"
          value={candsStore.luceneWithinMust}
          onChange={(e) => runInAction(() => { candsStore.luceneWithinMust = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
        <TextField
          fullWidth
          size="small"
          label="Should have — separate by comma"
          placeholder="e.g. electronics"
          value={candsStore.luceneWithinShould}
          onChange={(e) => runInAction(() => { candsStore.luceneWithinShould = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
      </Stack>

      <Stack direction="row" justifyContent="space-between" alignItems="center">
        <Button variant="text" color="secondary" onClick={handleClear}>
          Clear
        </Button>
        <Button
          variant="contained"
          color="primary"
          onClick={handleSearch}
          sx={{ px: 4 }}
          disabled={!candsStore.luceneFirstMust.trim() && !candsStore.luceneFirstShould.trim()}
        >
          Search
        </Button>
      </Stack>
    </Box>
  );
});

// ─────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────




function parseTerms(raw: string, occur: TermOccur): IComplexSearchTerm[] {
  return raw
    .split(",")
    .map((t) => t.trim())
    .filter(Boolean)
    .map((value) => ({
      value,
      occur,
      matchType: occur === "Must" && value.includes(" ") 
        ? "ExactPhrase" 
        : "Keyword",
    }));
}

const SectionLabel = ({ label }: { label: string }) => (
  <Divider sx={{ mb: 1.5 }}>
    <Typography
      variant="caption"
      color="text.secondary"
      fontWeight={600}
      sx={{ textTransform: "uppercase", letterSpacing: 0.5 }}
    >
      {label}
    </Typography>
  </Divider>
);