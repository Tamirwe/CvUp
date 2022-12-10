import { Box, Tab, Tabs, Typography } from "@mui/material";
import { useState } from "react";
import { CvsList } from "../cvs/CvsList";

interface IProps {
  tabSelected: number;
}

export const TabsDrawerLeft = ({ tabSelected }: IProps) => {
  const [value, setValue] = useState(tabSelected);

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  const handleCvClick = () => {};

  return (
    <Box sx={{ width: "100%" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={value}
          onChange={handleChange}
          aria-label="basic tabs example"
        >
          <Tab label="Positions" {...a11yProps(0)} />
          <Tab label="Candidates" {...a11yProps(1)} />
        </Tabs>
      </Box>
      <TabPanel value={value} index={0}>
        <CvsList />
      </TabPanel>
      <TabPanel value={value} index={1}></TabPanel>
    </Box>
  );
};

interface ITabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: ITabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    "aria-controls": `simple-tabpanel-${index}`,
  };
}
