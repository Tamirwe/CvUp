import {
  Box,
  Button,
  Checkbox,
  Divider,
  FormControl,
  FormControlLabel,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { runInAction } from "mobx";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { MdRefresh, MdRemove } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onClose: () => void;
  positionId?: number;
}

export const ComplexSearchForm = observer(({ onClose, positionId }: IProps) => {
  const { candsStore, generalStore } = useStore();
  const [selectedSearchId, setSelectedSearchId] = useState<number | "">("");

  useEffect(() => {
    generalStore.getSearchTermsList();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleGetKeywords = () => {
    if (positionId && positionId > 0) {
      candsStore.getPositionSearchTerms(positionId, true);
    }
  };

  const handleSelectSavedSearch = (id: number) => {
    setSelectedSearchId(id);
    candsStore.loadSearchTermsById(id);
  };

  const handleDeleteSavedSearch = (
    event: React.MouseEvent,
    id: number,
  ) => {
    event.stopPropagation();
    generalStore.deleteSearchTermsItem(id);

    if (selectedSearchId === id) {
      setSelectedSearchId("");
    }
  };

  const handleClear = () => {
    runInAction(() => {
      candsStore.searchTermsMustHave = "";
      candsStore.searchTermsShouldHave = "";
      candsStore.searchTermsMustHaveInResult = "";
      candsStore.searchTermsShouldHaveInResult = "";
      candsStore.searchTermsAiSearchPhrase = "";
      candsStore.searchTermsIsAiSearch = true;
      candsStore.searchTermsIsIndexSearch = true;
    });
  };

  const handleSearch = () => {
    candsStore.searchCandsByUiSearchForm();
    onClose();
  };

  return (
    <Box sx={{ direction: "rtl" }}>
      <SectionLabel label="Saved searches" />
      <Stack direction="row" spacing={1} alignItems="center" sx={{ mt: 2.5, mb: 2.5 }}>
        <IconButton
          title="Refresh"
          onClick={() => generalStore.getSearchTermsList(true)}
        >
          <MdRefresh />
        </IconButton>
        <FormControl fullWidth size="small" sx={{ direction: "rtl" }}>
          <InputLabel id="saved-search-label">Load saved search</InputLabel>
          <Select
            labelId="saved-search-label"
            label="Load saved search"
            value={selectedSearchId}
            onChange={(e) => handleSelectSavedSearch(Number(e.target.value))}
            renderValue={(value) =>
              generalStore.searchTermsList.find((x) => x.id === value)?.searchDescr ||
              `#${value}`
            }
          >
            {generalStore.searchTermsList.map((item) => (
              <MenuItem key={item.id} value={item.id} sx={{ direction: "rtl" }}>
                <Stack
                  direction="row"
                  justifyContent="space-between"
                  alignItems="center"
                  sx={{ width: "100%" }}
                >
                  <span>{item.searchDescr || `#${item.id}`}</span>
                  <IconButton
                    size="small"
                    title="Delete"
                    onClick={(e) => handleDeleteSavedSearch(e, item.id)}
                  >
                    <MdRemove />
                  </IconButton>
                </Stack>
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Stack>

      <SectionLabel label="Index Search" />
      <FormControlLabel
        control={
          <Checkbox
            id="isIndexSearch"
            checked={candsStore.searchTermsIsIndexSearch}
            onChange={(e) => runInAction(() => { candsStore.searchTermsIsIndexSearch = e.target.checked; })}
          />
        }
        label="Index search"
        sx={{ direction: "rtl", mr: 0 }}
      />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. React, TypeScript, Node"
          value={candsStore.searchTermsMustHave}
          onChange={(e) => runInAction(() => { candsStore.searchTermsMustHave = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
        <TextField
          fullWidth
          size="small"
          multiline
          rows={3}
          label="Should have — separate by comma"
          placeholder="e.g. AWS, Docker"
          value={candsStore.searchTermsShouldHave}
          onChange={(e) => runInAction(() => { candsStore.searchTermsShouldHave = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
      </Stack>

      <SectionLabel label="Index Search within results" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        <TextField
          fullWidth
          size="small"
          label="Must have — separate by comma"
          placeholder="e.g. clean rooms, automation"
          value={candsStore.searchTermsMustHaveInResult}
          onChange={(e) => runInAction(() => { candsStore.searchTermsMustHaveInResult = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
        <TextField
          fullWidth
          size="small"
          label="Should have — separate by comma"
          placeholder="e.g. electronics"
          value={candsStore.searchTermsShouldHaveInResult}
          onChange={(e) => runInAction(() => { candsStore.searchTermsShouldHaveInResult = e.target.value; })}
          sx={{ direction: "rtl" }}
        />
      </Stack>

      <SectionLabel label="AI search within results" />
      <FormControlLabel
        control={
          <Checkbox
            id="isAiSearch"
            checked={candsStore.searchTermsIsAiSearch}
            onChange={(e) => runInAction(() => { candsStore.searchTermsIsAiSearch = e.target.checked; })}
          />
        }
        label="AI search"
        sx={{ direction: "rtl", mr: 0 }}
      />
      <TextField
        fullWidth
        multiline
        rows={3}
        label="Describe the candidate you are looking for"
        placeholder="e.g. Senior C# developer with fintech experience and strong SQL skills..."
        value={candsStore.searchTermsAiSearchPhrase}
        onChange={(e) => runInAction(() => { candsStore.searchTermsAiSearchPhrase = e.target.value; })}
        sx={{ mb: 2.5, direction: "rtl" }}
        autoFocus
      />

      <Stack direction="row" justifyContent="space-between" alignItems="center">
        <Button variant="text" color="secondary" onClick={handleClear}>
          Clear
        </Button>
        {positionId !== undefined && positionId > 0 && (
          <Button variant="outlined" color="primary" onClick={handleGetKeywords} sx={{ mx: 1.5 }}>
            Get keywords
          </Button>
        )}
        <Button
          variant="contained"
          color="primary"
          onClick={handleSearch}
          sx={{ px: 4 }}
          disabled={!candsStore.searchTermsMustHave.trim() && !candsStore.searchTermsShouldHave.trim()}
        >
          Search
        </Button>
      </Stack>
    </Box>
  );
});

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
