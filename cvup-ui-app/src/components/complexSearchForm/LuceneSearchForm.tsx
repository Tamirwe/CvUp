import {
  Box,
  Button,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { IComplexSearchTerm, TermOccur } from "../../models/GeneralModels";
import { useStore } from "../../Hooks/useStore";

export const LuceneSearchForm = () => {
   const { candsStore } = useStore();
  const [firstMust, setFirstMust] = useState("");
  const [firstShould, setFirstShould] = useState("");
  const [withinMust, setWithinMust] = useState("");
  const [withinShould, setWithinShould] = useState("");

  const handleClear = () => {
    setFirstMust("");
    setFirstShould("");
    setWithinMust("");
    setWithinShould("");
  };

  const handleSearch = () => {
    const firstSearch: IComplexSearchTerm[] = [
      ...parseTerms(firstMust, "Must"),
      ...parseTerms(firstShould, "Should"),
    ];

    const withinTerms: IComplexSearchTerm[] = [
      ...parseTerms(withinMust, "Must"),
      ...parseTerms(withinShould, "Should"),
    ];

    const searchWithin = withinTerms.length > 0 ? withinTerms : undefined;

    candsStore.complexSearchCands(firstSearch, searchWithin);
  };

  return (
    <Box>
      <SectionLabel label="Search" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. React, TypeScript, Node"
          value={firstMust}
          onChange={(e) => setFirstMust(e.target.value)}
        />
        <TextField
          fullWidth
          size="small"
          label="Should have — separate by comma"
          placeholder="e.g. AWS, Docker"
          value={firstShould}
          onChange={(e) => setFirstShould(e.target.value)}
        />
      </Stack>

      <SectionLabel label="Search within results" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. clean rooms, automation"
          value={withinMust}
          onChange={(e) => setWithinMust(e.target.value)}
        />
        <TextField
          fullWidth
          size="small"
          label="Should have — separate by comma"
          placeholder="e.g. electronics"
          value={withinShould}
          onChange={(e) => setWithinShould(e.target.value)}
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
          disabled={!firstMust.trim() && !firstShould.trim()}
        >
          Search
        </Button>
      </Stack>
    </Box>
  );
};

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