const usfo = {
    mode: "standalone",
    ajaxUrl: "",
    userParams: {
        fontName: "Times New Roman",
        fontSize: 18,
        headingFontSize: 22,
        lineSpace: 1,
    },
    menuList: [],
    usfoIsActive: false,
    showInlineMenu: true,
    isAutoFormat: false,

    getCookie: function (cookieName) {
        return ("; " + document.cookie)
            .split(`; ${cookieName}=`)
            .pop()
            .split(";")[0];
    },
    setCookie: function (cookieName, cookieValue) {
        const d = new Date();
        d.setTime(d.getTime() + 30 * 24 * 60 * 60 * 1000);
        const expires = "expires=" + d.toUTCString();
        document.cookie =
            cookieName + "=" + cookieValue + ";" + expires + ";path=/";
    },

    alert: function (messageText) {
        const $overlay = $("<div>", {
            id: "usfo-message-box-overlay",
        }).appendTo("body");

        const $messageBox = $("<div>", {
            id: "usfo-message-box",
        }).appendTo($overlay);

        const $messageHeader = $("<div>", { 
            id: "usfo-message-header",
        }).appendTo($messageBox);

        $("<div>", {
            id: "usfo-message-header-text",
            text: "Usable & Formatted",
        }).appendTo($messageHeader);

        $("<div>", {
            id: "usfo-close-button",
            text: "×",
            click: usfo.closeAlert,
        }).appendTo($messageHeader);

        $("<div>", {
            id: "usfo-message-text",
            html: messageText,
        }).appendTo($messageBox);

        $overlay.fadeIn();
    },

    // Function to hide the message box
    closeAlert: function () {
        // Hide the message box overlay
        $("#usfo-message-box-overlay").fadeOut(function () {
            $("#usfo-message-box-overlay").remove();
        });
    },

    updatePageContent: function () {
        $("#page-content .no-overflow p, #page-content .no-overflow div").each(
            function () {
                $(this).css({
                    "font-size": (usfo.userParams.fontSize || 16) + "px",
                    "font-family": usfo.userParams.fontName,
                });
            }
        );
        $(
            "#page-content .no-overflow h3, #page-content .no-overflow h4, #page-content .no-overflow h5"
        ).each(function () {
            $(this).css({
                "font-size": (usfo.userParams.headingFontSize || 20) + "px",
                "font-family": usfo.userParams.fontName,
            });
        });
    },

    // Download document with added extension "usfo" (app need to register this extension)
    downloadUsfoDocument: function () {
        const link = $(this).data("link");
        const oReq = new XMLHttpRequest();
        oReq.open("GET", link, true);
        oReq.responseType = "arraybuffer";
        oReq.onload = function (oEvent) {
            const arrayBuffer = oReq.response;
            const urlParts = oEvent.target.responseURL.split("/");
            const docFileName =
                urlParts[urlParts.length - 1].split("?")[0] + ".usfo";
            const blob = new Blob([arrayBuffer], {
                type: "application/octet-stream",
            });
            const objectUrl = URL.createObjectURL(blob);
            const downloadURL = function (data, fileName) {
                const a = document.createElement("a");
                a.href = data;
                a.download = fileName;
                document.body.appendChild(a);
                a.style = "display: none";
                a.click();
                a.remove();
            };
            downloadURL(objectUrl, docFileName);
            setTimeout(function () {
                return window.URL.revokeObjectURL(objectUrl);
            }, 1000);
        };
        oReq.send();
    },

    // Download document through WebSocket stream
    streamUsfoDocument: function () {
        const $this = $(this);
        const link = $this.data("link");

        const wsUri = "ws://127.0.0.1:18080/";
        const websocket = new WebSocket(wsUri);
        function doSend(data) {
            console.log(`SEND: ${data.length}`);
            websocket.send(data);
            websocket.close();
        }
        websocket.onopen = () => {
            console.log("CONNECTED");
        };
        websocket.onclose = () => {
            console.log("DISCONNECTED");
        };

        websocket.onmessage = (e) => {
            console.log("RESPONSE:", e.data);
        };
        websocket.onerror = (e) => {
            console.error("Error", e);
            websocket.close();
            setTimeout(() => {
                usfo.alert(
                    "Lai izmantotu šo funkciju, jābūt aktīvai lietotnei Usable & Formatted!"
                );
            }, 100);
        };

        const oReq = new XMLHttpRequest();
        oReq.open("GET", link, true);
        oReq.responseType = "arraybuffer";
        oReq.onload = function (oEvent) {
            const arrayBuffer = oReq.response;
            const urlParts = oEvent.target.responseURL.split("/");
            const docFileName = urlParts[urlParts.length - 1].split("?")[0];
            const extension = docFileName.split(".").pop();
            if (
                extension !== "docx" &&
                extension !== "doc" &&
                extension !== "odt" &&
                extension !== "pdf"
            ) {
                return;
            }
            const fileNameBytes = new TextEncoder().encode(docFileName + "\n");
            const combinedBytes = new Uint8Array(
                fileNameBytes.length + arrayBuffer.byteLength
            );
            combinedBytes.set(fileNameBytes, 0);
            combinedBytes.set(
                new Uint8Array(arrayBuffer),
                fileNameBytes.length
            );
            doSend(combinedBytes);
        };
        oReq.send();
    },

    readUserParams: function () {
        usfo.userParams.fontName =
            usfo.getCookie("usfoFormat_fontName") || "Times New Roman";
        usfo.userParams.fontSize = usfo.getCookie("usfoFormat_fontSize") || 18;
        usfo.userParams.headingFontSize =
            usfo.getCookie("usfoFormat_headingFontSize") || 22;

        usfo.isAutoFormat = +usfo.getCookie("usfoAutoFormat") === 1;
    },

    setUserParam: function (action, value) {
        usfo.userParams[action] = value;
        usfo.setCookie("usfoFormat_" + action, value);
        usfo.updateActiveMenuItems();
        usfo.updatePageContent();
    },

    onDropDownAction: function (action, value) {
        $(".dropdown-submenu a.usfo-submenu").next("ul").hide();
        switch (action) {
            case "format":
                usfo.updatePageContent();
                return;
            case "autoformat":
                usfo.toggleAutoFormat();
                return;
            case "fontName":
            case "fontSize":
            case "headingFontSize":
                usfo.setUserParam(action, value);
                return;
            case "about":
                usfo.alert('Lasīšanas atvieglošanas rīks, izstrādāts projekta EduAim ietvaros.<br/>' +
                    '&quot;Augstskolu digitālās kapacitātes celšana ar tiešsaistes mācību resursu un analītikas viedu integrāciju&quot;' +
                    ' Projekts: 8.2.3.0/22/A/003 ESF (REACT-EU)' +
                    '<br/><a href="https://www.eduaim.eu/tools#UsableFormatted" target="_blank">Plašāka informācija EduAim lapā.</a>');
                return;
        }
    },

    toggleAutoFormat: function () {
        usfo.isAutoFormat = !usfo.isAutoFormat;
        usfo.setCookie("usfoAutoFormat", usfo.isAutoFormat ? 1 : 0);
        usfo.updateAutoFormat();
    },

    updateAutoFormat: function () {
        $(".usfo-autoformat").toggleClass("active", usfo.isAutoFormat);
        if (usfo.isAutoFormat && usfo.usfoIsActive) {
            usfo.updatePageContent();
        }
    },

    addHeaderScripts: function () {
        $("head").append(
            "<link href='https://fonts.googleapis.com/css?family=Inter' rel='stylesheet'>"
        );
        const style =
            "a.usfo-action,a.usfo-submenu{margin:0 15px 0 15px;white-space:nowrap;} " +
            'a.usfo-action.active:before{font-family:FontAwesome;content:"\uf00c";font-size:.75rem;padding-left:.25rem;} ' +
            "a.usfo-action.active{margin-left:0;}" +
            ".dropdown-submenu{position: relative;} .dropdown-submenu .dropdown-menu{top:0;left:100%;margin-top:-1px;}" +
            "#usfo-message-box-overlay {display: none;position: fixed;top: 0;left: 0;width: 100%;height: 100%;background: rgba(0, 0, 0, 0.3);" +
            "display: flex;justify-content: center;align-items: center;z-index: 9999;}" +
            "#usfo-message-box{background:#fff;border:1px solid #235b8e;border-radius:10px;max-width:800px;text-align:center;position:relative;}" +
            "#usfo-close-button {position:absolute;top:0;right:10px;cursor:pointer;color:white;}" +
            "#usfo-message-header{background-color:#235b8e;position:relative;border-radius:9px 9px 0 0;}" +
            "#usfo-message-header-text{color:white;}" +
            "#usfo-message-text{margin:10px;}"
            ;
        $("head").append('<style type="text/css">' + style + "</style>");
    },

    drawTopButton: function () {
        const $faicon = $("<i class='fa fa-toggle-off'></i>");
        const $ico = $("<img/>")
            .attr("src", usfo.ajaxUrl + "/favicon-32x32.png")
            .css({ marginRight: "2px" });
        const $subdiv = $("<div/>")
            .addClass("popover-region-toggle nav-link icon-no-margin")
            .append($ico)
            .append($faicon);
        const $div = $("<div/>")
            .addClass("popover-region")
            .attr("id", "usfoToggleBtn")
            .attr("title", "Usable & Formatted")
            .append($subdiv)
            .click(usfo.onTopButtonClick);
        $("#usernavigation").prepend($div);
    },

    onTopButtonClick: function () {
        usfo.usfoIsActive = !usfo.usfoIsActive;
        document.cookie =
            "usfoStatus=" + (usfo.usfoIsActive ? 1 : 0) + "; path=/";
        usfo.setUsfoStatus();
    },

    readUsfoStatus: function () {
        usfo.usfoIsActive = +usfo.getCookie("usfoStatus") == 1;
    },

    setUsfoStatus: function () {
        $("#usfoToggleBtn i")
            .removeClass("fa-toggle-off fa-toggle-on")
            .addClass(usfo.usfoIsActive ? "fa-toggle-on" : "fa-toggle-off");
        if (usfo.usfoIsActive) {
            usfo.addDocumentAnchors();
        } else {
            usfo.removeDocumentAnchors();
        }
        $(".usfo-menu-btn").css({
            visibility: usfo.usfoIsActive ? "visible" : "hidden",
        });
    },

    updateActiveMenuItems: function () {
        for (let i in usfo.userParams) {
            const item = usfo.userParams[i];
            $(".usfo-mainmenu[data-action=" + i + "] a.active").removeClass(
                "active"
            );
            $(
                ".usfo-mainmenu[data-action=" +
                    i +
                    "] a[data-itemvalue='" +
                    item +
                    "']"
            ).addClass("active");
        }
    },

    drawMenuItems: function () {
        const formatButton = $("<div/>")
            .append(
                $("<img/>").attr("src", usfo.ajaxUrl + "/favicon-32x32.png")
            )
            .addClass(
                "content activityiconcontainer modicon_page usfo-menu-btn"
            )
            .css({
                color: "white",
                "margin-left": "10px",
                width: "40px",
                height: "40px",
                visibility: "hidden",
            })
            .attr("title", "Usable & Formatted")
            .click(usfo.updatePageContent);
        const dropDownButton = $("<button/>")
            .addClass(
                "content activityiconcontainer modicon_page btn btn-default dropdown-toggle"
            )
            .css({
                color: "white",
                width: "10px",
                height: "40px",
                "margin-left": "1px",
                "border-left-color": "white",
                "border-top-left-radius": 0,
                "border-bottom-left-radius": 0,
            })
            .attr("type", "button")
            .attr("title", "Format & usable")
            .attr("data-toggle", "dropdown");
        const menus = [];
        for (let i in usfo.menuList) {
            const item = usfo.menuList[i];
            let submenu = null;
            if (item.items?.length) {
                const submentuItems = [];
                for (let j in item.items) {
                    const subitem = item.items[j];
                    const a = $("<a/>")
                        .text(subitem.title)
                        .attr("tabindex", "-1")
                        .attr("href", "#")
                        .attr("data-itemvalue", subitem.ic)
                        .attr("data-itemaction", item.ic)
                        .addClass("usfo-action");
                    submentuItems.push($("<li/>").append(a));
                }
                submenu = [
                    $("<a/>")
                        .text(item.title)
                        .attr("tabindex", "-1")
                        .attr("href", "#")
                        .addClass("usfo-submenu"),
                    $("<ul/>").html(submentuItems).addClass("dropdown-menu"),
                ];
                const menuItem = $("<li/>")
                    .append(submenu)
                    .attr("data-action", item.ic)
                    .addClass("dropdown-submenu usfo-mainmenu");
                menus.push(menuItem);
            }
        }
        menus.push(
            $(
                "<li><a tabindex='-1' href='#' class='usfo-action' data-itemaction='format'>Formatēt</a></li>"
            )
        );
        menus.push(
            $(
                "<li><a tabindex='-1' href='#' class='usfo-action usfo-autoformat' data-itemaction='autoformat'>Formatēt automātiski</a></li>"
            )
        );
        menus.push(
            $(
                "<li><a tabindex='-1' href='#' class='usfo-action' data-itemaction='about'>Par Usable &amp; Formatted</a></li>"
            )
        );
        menus.push(
            $(
                "<li><a tabindex='-1' href='https://www.eduaim.eu/tools#UsableFormatted' class='usfo-action' target='_blank'>Par EduAim</a></li>"
            )
        );

        const dropdownMenu = $("<div/>")
            .addClass("dropdown usfo-menu-btn")
            .css({
                visibility: "hidden",
            })
            .html([
                dropDownButton,
                $("<ul/>").addClass("dropdown-menu").html(menus),
            ]);

        $("#page-header .page-context-header")
            .append(formatButton)
            .append(dropdownMenu);

        $(".page-context-header").css({ overflow: "visible" });

        $(".dropdown-submenu a.usfo-submenu").on("click", function (e) {
            $(".usfo-mainmenu ul").hide();
            $(this).next("ul").show();
            e.stopPropagation();
            e.preventDefault();
        });
        $(".usfo-action").on("click", function () {
            const $this = $(this);
            const action = $this.data("itemaction");
            const value = $this.data("itemvalue");
            usfo.onDropDownAction(action, value);
        });
        usfo.updateActiveMenuItems();
    },

    addDocumentAnchors: function () {
        $(".activity-item .activity-instance ").each(function () {
            const $this = $(this);
            const $a = $this.find(".modtype_resource .activityname a");
            let oc = "";
            const onclickEvent = $a.attr("onclick");
            if (onclickEvent && onclickEvent.indexOf("window.open") > -1) {
                const match = onclickEvent.match(/window\.open\('([^']+)'/);
                oc = (!!match && match[1]) || "";
            }

            const href = oc || $a.attr("href");
            if (!href || !/mod\/resource\/view\.php/.test(href)) return;

            const $ico = $("<img/>")
                .attr("src", usfo.ajaxUrl + "/favicon-32x32.png")
                .css({ width: 20, height: 20 });

            const $dpButton = $("<div/>")
                .append($ico)
                .addClass(
                    "content activityiconcontainer modicon_page usfo-button"
                )
                .css({
                    color: "white",
                    width: "24px",
                    height: "24px",
                    "align-self": "end",
                    "margin-left": "10px",
                    cursor: "pointer",
                })
                .attr("title", "Usable & Formatted")
                .attr("data-link", href)
                .click(usfo.streamUsfoDocument);
            $this.after($dpButton);
        });
    },

    removeDocumentAnchors: function () {
        $(".usfo-button").remove();
    },

    initStaticValues: function () {
        const fonts = [
            "Arial",
            "Calibri",
            "Cambria",
            "Cambria Math",
            "Candara",
            "Comic Sans MS",
            "Consolas",
            "Constantia",
            "Corbel",
            "Courier New",
            "Ebrima",
            "Franklin Gothic Medium",
            "Gabriola",
            "Gadugi",
            "Georgia",
            "HoloLens MDL2 Assets",
            "Impact",
            "Inter",
            "Lucida Console",
            "Lucida Sans Unicode",
            "Microsoft Sans Serif",
            "Segoe UI",
            "Segoe UI Light",
            "Segoe UI Semibold",
            "Segoe UI Symbol",
            "Tahoma",
            "Times New Roman",
            "Trebuchet MS",
            "Verdana",
        ];
        const fontSizes = [
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72",
        ];
        usfo.menuList = [
            {
                ic: "fontName",
                title: "Fonts",
                items: fonts.map((x) => ({ ic: x, title: x })),
            },
            {
                ic: "fontSize",
                title: "Fonta izmērs",
                items: fontSizes.map((x) => ({ ic: x, title: x })),
            },
            {
                ic: "headingFontSize",
                title: "Virsrakstu fonta izmērs",
                items: fontSizes.map((x) => ({ ic: x, title: x })),
            },
        ];
    },

    init: function (fuScriptUrl) {
        usfo.addHeaderScripts();
        //Remove script from URL
        usfo.ajaxUrl =
            fuScriptUrl.protocol +
            "//" +
            fuScriptUrl.host +
            fuScriptUrl.pathname.replace(/\/[^/]+$/, "");
        if (usfo.mode === "standalone") {
            //V1
            usfo.initStaticValues();
            usfo.readUserParams();
            usfo.drawMenuItems();
        } else {
            //V2
            $.getJSON(usfo.ajaxUrl + "/initParams", function (data) {
                usfo.userParams = data?.settings || {};
                usfo.menuList = data?.menuList || [];
                usfo.drawMenuItems();
            });
        }
        usfo.drawTopButton();
        usfo.readUsfoStatus();
        usfo.setUsfoStatus();
        usfo.updateAutoFormat();
    },

    start: function () {
        document.cookie =
            "usfoIsEnabled=1; expires=Thu, 01 Jan 2026 00:00:00 GMT; path=/";
    },
    stop: function () {
        document.cookie =
            "usfoIsEnabled=0; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
    },
};

const fuScriptUrl = new URL(document.currentScript.src);

if (+usfo.getCookie("usfoIsEnabled") === 1) {
    (function () {
        usfo.jqTimer = setInterval(function () {
            if (window.jQuery) {
                clearInterval(usfo.jqTimer);
                usfo.init(fuScriptUrl);
            }
        }, 100);
    })();
}
