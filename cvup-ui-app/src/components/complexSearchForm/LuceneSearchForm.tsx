import {
  Autocomplete,
  Box,
  Button,
  Divider,
  IconButton,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { runInAction } from "mobx";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { MdRefresh, MdRemove } from "react-icons/md";
import { ISearchTermsListItem } from "../../models/GeneralModels";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  onClose: () => void;
  positionId?: number;
}

export const LuceneSearchForm = observer(({ onClose, positionId }: IProps) => {
  const { candsStore, generalStore } = useStore();

  useEffect(() => {
    generalStore.getSearchTermsList();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleGetKeywords = () => {
    if (positionId && positionId > 0) {
      candsStore.getPositionSearchTerms(positionId, true);
    }
  };

  const handleSelectSavedSearch = (item: ISearchTermsListItem | null) => {
    if (item) {
      candsStore.loadSearchTermsById(item.id);
    }
  };

  const handleDeleteSavedSearch = (
    event: React.MouseEvent,
    id: number,
  ) => {
    event.stopPropagation();
    generalStore.deleteSearchTermsItem(id);
  };

  const handleClear = () => {
    runInAction(() => {
      candsStore.searchTermsMustHave = "";
      candsStore.searchTermsShouldHave = "";
      candsStore.searchTermsMustHaveInResult = "";
      candsStore.searchTermsShouldHaveInResult = "";
      candsStore.searchTermsAiSearchPhrase = "";
    });
  };

  const handleSearch = () => {
    candsStore.searchCandsByUiSearchForm();
    onClose();
  };

  return (
    <Box sx={{ direction: "rtl" }}>
      <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 2.5 }}>
        <Autocomplete
          fullWidth
          size="small"
          options={generalStore.searchTermsList}
          getOptionLabel={(option) => option.searchDescr || `#${option.id}`}
          value={null}
          onChange={(_, value) => handleSelectSavedSearch(value)}
          isOptionEqualToValue={(option, value) => option.id === value.id}
          renderOption={(props, option) => (
            <li {...props} key={option.id}>
              <Stack
                direction="row"
                justifyContent="space-between"
                alignItems="center"
                sx={{ width: "100%" }}
              >
                <span>{option.searchDescr || `#${option.id}`}</span>
                <IconButton
                  size="small"
                  title="Delete"
                  onClick={(e) => handleDeleteSavedSearch(e, option.id)}
                >
                  <MdRemove />
                </IconButton>
              </Stack>
            </li>
          )}
          renderInput={(params) => (
            <TextField {...params} label="Load saved search" sx={{ direction: "rtl" }} />
          )}
          sx={{ direction: "rtl" }}
        />
        <IconButton
          title="Refresh"
          onClick={() => generalStore.getSearchTermsList(true)}
        >
          <MdRefresh />
        </IconButton>
      </Stack>

      <SectionLabel label="Index Search" />
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
      <TextField
        fullWidth
        multiline
        rows={4}
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