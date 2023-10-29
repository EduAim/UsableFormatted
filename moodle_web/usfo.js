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
    usfoOnOffTimeout: null,

    updatePageContent: function () {
        console.log(usfo.userParams);
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

    downloadUsfoDocument: function () {
        const link = $(this).data("link");
        const oReq = new XMLHttpRequest();
        oReq.open("GET", link, true);
        oReq.responseType = "arraybuffer";
        oReq.onload = function (oEvent) {
            const arrayBuffer = oReq.response;
            const urlParts = oEvent.target.responseURL.split("/");
            const docFileName = urlParts[urlParts.length - 1] + ".usfo";
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

    streamUsfoDocument_: function () {
        const link = $(this).data("link");
        console.log("streamUsfoDocument_", link);
        const wsUri = "ws://127.0.0.1:18080/";
        const websocket = new WebSocket(wsUri);
        function doSend(message) {
            console.log(`SENT: ${message}`);
            websocket.send(message);
            console.log("try close");
            websocket.close();
        }
        websocket.onopen = (e) => {
            console.log("CONNECTED");
            doSend(link);
        };
        websocket.onclose = (e) => {
            console.log("DISCONNECTED");
        };

        websocket.onmessage = (e) => {
            console.log("RESPONSE:", e.data);
        };
        websocket.onerror = (e) => {
            console.error("Error", e);
        };
    },

    streamUsfoDocument: function () {
        const link = $(this).data("link");

        const wsUri = "ws://127.0.0.1:18080/";
        const websocket = new WebSocket(wsUri);
        function doSend(data) {
            console.log(`SEND: ${data.length}`);
            websocket.send(data);
            console.log("try close");
            websocket.close();
        }
        websocket.onopen = (e) => {
            console.log("CONNECTED");
            //doSend(link);
        };
        websocket.onclose = (e) => {
            console.log("DISCONNECTED");
        };

        websocket.onmessage = (e) => {
            console.log("RESPONSE:", e.data);
        };
        websocket.onerror = (e) => {
            console.error("Error", e);
        };


        const oReq = new XMLHttpRequest();
        oReq.open("GET", link, true);
        oReq.responseType = "arraybuffer";
        oReq.onload = function (oEvent) {
            const arrayBuffer = oReq.response;
            const urlParts = oEvent.target.responseURL.split("/");
            const docFileName = urlParts[urlParts.length - 1];
            console.log("docFileName", docFileName);
            const fileNameBytes = new TextEncoder().encode(docFileName + "\n");
            const combinedBytes = new Uint8Array(fileNameBytes.length + arrayBuffer.byteLength);
            combinedBytes.set(fileNameBytes, 0);
            combinedBytes.set(new Uint8Array(arrayBuffer), fileNameBytes.length);
            doSend(combinedBytes);
        };
        oReq.send();
    },

    setUserParam: function (action, value) {
        console.log(
            "setUserParam",
            action,
            value,
            usfo.ajaxUrl + "/setUserValue"
        );
        usfo.userParams[action] = value;
        $.post(
            usfo.ajaxUrl + "/setUserValue",
            { ic: action, value: value },
            function () {
                console.log("post response");
            }
        )
            .fail(function (e) {
                console.log("post fail", e);
            })
            .done(function () {
                console.log("post done");
            });
        usfo.updatePageContent();
    },

    onDropDownAction: function (action, value) {
        $(".dropdown-submenu a.fu-submenu").next("ul").hide();
        switch (action) {
            case "format":
                usfo.updatePageContent();
                return;
            case "fontName":
            case "fontSize":
            case "headingFontSize":
                usfo.setUserParam(action, value);
                return;
        }
    },

    addHeaderScripts: function () {
        $("head").append(
            "<link href='https://fonts.googleapis.com/css?family=Inter' rel='stylesheet'>"
        );
        $("head").append(
            '<style type="text/css">.dropdown-submenu{position: relative;} .dropdown-submenu .dropdown-menu{top:0;left:100%;margin-top:-1px;}</style>'
        );
    },

    drawTopButton: function () {
        const $faicon = $("<i class='fa fa-toggle-off'></i>");
        const $ico = $("<img/>")
            .attr("src", usfo.ajaxUrl + "/favicon-16x16.png")
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
        $("#usfoToggleBtn i")
            .removeClass("fa-toggle-off fa-toggle-on")
            .addClass(usfo.usfoIsActive ? "fa-toggle-on" : "fa-toggle-off");
        clearTimeout(usfo.usfoOnOffTimeout);
        usfo.usfoOnOffTimeout = setTimeout(function () {}, 2000);
    },

    drawMenuItems: function () {
        const formatButton = $("<div/>")
            .html("<i class='fa fa-binoculars'></i>")
            .addClass("content activityiconcontainer modicon_page")
            .css({
                color: "white",
                "margin-left": "10px",
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
                        .addClass("fu-action");
                    submentuItems.push($("<li/>").html(a));
                }
                submenu = [
                    $("<a/>")
                        .text(item.title)
                        .attr("tabindex", "-1")
                        .attr("href", "#")
                        .addClass("fu-submenu"),
                    $("<ul/>").html(submentuItems).addClass("dropdown-menu"),
                ];
                const menuItem = $("<li/>")
                    .html(submenu)
                    .addClass("dropdown-submenu");
                menus.push(menuItem);
            }
        }
        menus.push(
            $(
                "<li><a tabindex='-1' href='#' data-itemaction='format'>Formatēt</a></li>"
            )
        );
        const dropdownMenu = $("<div/>")
            .addClass("dropdown")
            .html([
                dropDownButton,
                $("<ul/>").addClass("dropdown-menu").html(menus),
            ]);

        $("#page-header .page-context-header")
            .append(formatButton)
            .append(dropdownMenu);

        $(".page-context-header").css({ overflow: "visible" });

        $(".dropdown-submenu a.fu-submenu").on("click", function (e) {
            $(this).next("ul").toggle();
            e.stopPropagation();
            e.preventDefault();
        });
        $(".fu-action").on("click", function () {
            const $this = $(this);
            const action = $this.data("itemaction");
            const value = $this.data("itemvalue");
            usfo.onDropDownAction(action, value);
        });
    },

    addDocumentAnchors: function () {
        $(".activity-item .activity-instance ").each(function () {
            const $this = $(this);
            const href = $this
                .find(".modtype_resource .activityname a")
                .attr("href");
            if (!href || !/mod\/resource\/view\.php/.test(href)) return;

            const $dpButton = $("<div/>")
                .html("<i class='fa fa-file-word-o'></i>")
                .addClass("content activityiconcontainer modicon_page")
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
                // .click(usfo.downloadUsfoDocument);
                .click(usfo.streamUsfoDocument);
            $this.after($dpButton);
        });
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
        } else {
            //V2
            $.getJSON(usfo.ajaxUrl + "/initParams", function (data) {
                usfo.userParams = data?.settings || {};
                usfo.menuList = data?.menuList || [];
                usfo.drawMenuItems();
            });
        }
        usfo.drawTopButton();
        usfo.drawMenuItems();
        usfo.addDocumentAnchors();
    },
};

const fuScriptUrl = new URL(document.currentScript.src);

(function () {
    usfo.jqTimer = setInterval(function () {
        if (window.jQuery) {
            clearInterval(usfo.jqTimer);
            usfo.init(fuScriptUrl);
        }
    }, 100);
})();
