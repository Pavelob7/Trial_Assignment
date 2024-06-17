// RouteGraph.styles.js
import { styled } from '@mui/system';
import { Box, Button, ButtonGroup } from '@mui/material';

export const ButtonContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    justifyContent: 'center',
    gap: theme.spacing(5),
    marginBottom: theme.spacing(2),
}));

export const DateButtonGroup = styled(ButtonGroup)(({ theme }) => ({
    display: 'flex',
    justifyContent: 'center',
    gap: theme.spacing(2),
    marginBottom: theme.spacing(2),
}));

export const StyledButton = styled(Button)(({ theme }) => ({
    flex: 1,
}));
