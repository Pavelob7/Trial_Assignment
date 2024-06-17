// RouteGraph.utils.js
export const getSurfaceColor = (surface) => {
    switch (surface) {
        case 'ASPHALT':
            return "rgba(30,30,30,0.2)";
        case 'GROUND':
            return "rgba(12,255,0,0.2)";
        case 'SAND':
            return "rgba(255,241,0,0.2)";
        default:
            throw new Error('не указана поверхность');
    }
};

export const getSpeedColor = (speed) => {
    switch (speed) {
        case 'FAST':
            return "rgba(255,0,0,1)";
        case 'NORMAL':
            return "rgb(255,196,0)";
        case 'SLOW':
            return "rgb(0,111,255)";
        default:
            throw new Error('не указана скорость');
    }
};

export const formatUnixTime = (timestamp) => {
    const date = new Date(timestamp * 1000);
    return date.toLocaleString();
};
