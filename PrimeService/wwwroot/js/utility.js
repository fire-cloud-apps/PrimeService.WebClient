window.getWindowDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
    //Ref: https://www.syncfusion.com/faq/blazor/javascript-interop/how-do-i-get-the-window-dimension-or-size-in-blazor-webassembly
};

function InitalizeBarCode(){
    
    JsBarcode("#barcode", "1234", {
        lineColor: "#0aa",
        width: 4,
        height: 40,
        displayValue: false
    });
    console.warn("Barcode JsBarcode");
    return true;
}

function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}
function printProduct(data){
    var a = window.open('', '', 'height=500, width=500');
    a.document.write('<html>');
    a.document.write('<body > <br>');
    a.document.write('Product Details as HTML : ' +  data);
    a.document.write('</body></html>');
    a.document.close();
    a.print();
    return true;
}

function loadJs(sourceUrl) {
    if (sourceUrl.Length == 0) {
        console.error("Invalid source URL");
        return;
    }

    var tag = document.createElement('script');
    tag.src = sourceUrl;
    tag.type = "text/javascript";

    tag.onload = function () {
        console.log("Script loaded successfully");
    }

    tag.onerror = function () {
        console.error("Failed to load script");
    }

    document.body.appendChild(tag);
}