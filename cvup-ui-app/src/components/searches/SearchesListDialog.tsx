import { Dialog, DialogContent, IconButton, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { SearchesList } from "./SearchesList";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";
import { SearchControl } from "../header/SearchControl";
import { useStore } from "../../Hooks/useStore";
import { ISearchModel } from "../../models/GeneralModels";
import { MdStarOutline } from "react-icons/md";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export const SearchesListDialog = ({ isOpen, onClose }: IProps) => {
  const { candsStore } = useStore();
  const [open, setOpen] = useState(false);

  useEffect(() => {
    setOpen(isOpen);
  }, [isOpen]);

  const handleApplySearch = (searchVals: ISearchModel) => {
    candsStore.extSearch = searchVals;
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth={"xs"}>
      <BootstrapDialogTitle id="dialog-title" onClose={() => onClose()}>
        Candidate Searches
      </BootstrapDialogTitle>
      <DialogContent>
        <Stack direction="row" sx={{ width: "100%" }}>
          <IconButton
            title="Saved Searches"
            // onClick={() =>
            //   (generalStore.showSearchesListDialog =
            //     !generalStore.showSearchesListDialog)
            // }
          >
            <MdStarOutline />
          </IconButton>
          <SearchControl
            onSearch={(searchVals) => {
              candsStore.findSearches(searchVals);
            }}
            showRefreshList={true}
            onRefreshLists={() => candsStore.getSearches()}
          />
        </Stack>
        <SearchesList
          onApplySearch={(searchVals) => handleApplySearch(searchVals)}
        />
      </DialogContent>
    </Dialog>
  );
};
