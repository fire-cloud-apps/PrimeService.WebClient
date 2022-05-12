window.getWindowDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
    //Ref: https://www.syncfusion.com/faq/blazor/javascript-interop/how-do-i-get-the-window-dimension-or-size-in-blazor-webassembly
};

function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}