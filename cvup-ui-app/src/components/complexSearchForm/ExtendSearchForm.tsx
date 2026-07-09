import {
  Autocomplete,
  Box,
  Button,
  Chip,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { runInAction } from "mobx";
import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";

interface IProps {
  positionId: number;
  onClose: () => void;
}

export const ExtendSearchForm = observer(({ positionId, onClose }: IProps) => {
  const { candsStore } = useStore();

  useEffect(() => {
    candsStore.loadAnalyzedPosition(positionId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [positionId]);

  const handleSearch = () => {
    candsStore.extendSearchCands();
    onClose();
  };

  const chipField = (
    label: string,
    value: string[],
    setValue: (v: string[]) => void
  ) => (
    <Autocomplete
      multiple
      freeSolo
      size="small"
      options={[]}
      value={value}
      onChange={(_, newValue) =>
        runInAction(() => setValue(newValue as string[]))
      }
      renderTags={(tagValue, getTagProps) =>
        tagValue.map((option, index) => (
          <Chip variant="outlined" label={option} {...getTagProps({ index })} />
        ))
      }
      renderInput={(params) => (
        <TextField {...params} label={label} placeholder="Add and press Enter" sx={{ direction: "rtl" }} />
      )}
    />
  );

  return (
    <Box sx={{ direction: "rtl" }}>
      <SectionLabel label="Position" />
      <Stack spacing={0.5} sx={{ mb: 2 }}>
        <Typography variant="body2" color="text.secondary">
          <b>Title:</b> {candsStore.analyzedTitle || "—"} &nbsp;|&nbsp;
          <b>Seniority:</b> {candsStore.analyzedSeniority || "—"} &nbsp;|&nbsp;
          <b>Min years:</b> {candsStore.analyzedMinYearsExperience ?? "—"} &nbsp;|&nbsp;
          <b>Degree:</b> {candsStore.analyzedDegreeRequired || "—"}
        </Typography>
      </Stack>

      <SectionLabel label="Hard Requirements" />
      <Box sx={{ mb: 2 }}>
        {chipField("Hard requirements", candsStore.extendHardRequirements, (v) => runInAction(() => { candsStore.extendHardRequirements = v; }))}
      </Box>

      <SectionLabel label="Skills" />
      <Stack spacing={1.5} sx={{ mb: 2 }}>
        {chipField("Required skills", candsStore.extendSkillsRequired, (v) => runInAction(() => { candsStore.extendSkillsRequired = v; }))}
        {chipField("Preferred skills", candsStore.extendSkillsPreferred, (v) => runInAction(() => { candsStore.extendSkillsPreferred = v; }))}
      </Stack>

      <SectionLabel label="Industries" />
      <Box sx={{ mb: 2 }}>
        {chipField("Industries", candsStore.extendIndustries, (v) => runInAction(() => { candsStore.extendIndustries = v; }))}
      </Box>

      <SectionLabel label="Languages" />
      <Box sx={{ mb: 2 }}>
        {chipField("Languages", candsStore.extendLanguages, (v) => runInAction(() => { candsStore.extendLanguages = v; }))}
      </Box>

      <SectionLabel label="Lucene Keywords" />
      <Stack spacing={1.5} sx={{ mb: 3 }}>
        {chipField("English keywords", candsStore.extendKeywordsEn, (v) => runInAction(() => { candsStore.extendKeywordsEn = v; }))}
        {chipField("מילות מפתח בעברית", candsStore.extendKeywordsHe, (v) => runInAction(() => { candsStore.extendKeywordsHe = v; }))}
      </Stack>

      <Stack direction="row" justifyContent="flex-end">
        <Button variant="contained" color="primary" onClick={handleSearch} sx={{ px: 4 }}>
          Search
        </Button>
      </Stack>
    </Box>
  );
});

const SectionLabel = ({ label }: { label: string }) => (
  <Divider sx={{ mb: 1.5 }}>
    <Typography variant="caption" color="text.secondary" fontWeight={600} sx={{ textTransform: "uppercase", letterSpacing: 0.5 }}>
      {label}
    </Typography>
  </Divider>
);