// Crockford's supplant method (poor man's templating)
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function() {

    $('#side-menu').metisMenu();

});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        var topOffset = 50;
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        var height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    var element = $('ul.nav a').filter(function() {
        return this.href == url;
    }).addClass('active').parent().parent().addClass('in').parent();
    if (element.is('li')) {
        element.addClass('active');
    }
});

$(function () {
    var myhub = $.connection.cPostHub,
        $dropdwnmsg = $('#drpdwnmsg'),
        $topnavmsg = $('#topnavmsg'),
        $topnavtask = $('#topnavtask'),
        $noofmsg = $('#noofmsg'),
        $runtask = $('#runtask'),
        $fintask = $('#fintask'),
        $notiflistgrp = $('#notiflistgrp'),
        $failedtask = $('#failedtask');

    function init() {
    }

    // Add client-side hub methods that the server will call
    $.extend(myhub.client, {
        updateMessages: function (connId, msg, msgcnt, taskcnt, taskdone, taskfailed)
        {
            $topnavmsg.html('Task Messages -> ' + msgcnt)
            $taskselect = '.' + connId
            $noofmsg.html(msgcnt)
            $topnavtask.html('Running Tasks -> ' + (taskcnt - taskdone - taskfailed))
            $runtask.html((taskcnt - taskdone - taskfailed))
            $fintask.html(taskdone)
            $failedtask.html(taskfailed)
            //if ($($taskselect).length > 0) {
            //    $($taskselect).html(msg)
            //    $notiflistgrp.html(msg)
            //}
            //else
            //{
                $dropdwnmsg.append('<li class="' + connId
                    + '"><div>' + msg + '</div></li>')
                $notiflistgrp.append('<a class="list-group-item "'+ connId +'><i class="fa fa-tasks fa-fw"></i>'+msg)
            //}
        }
    });

    // Start the connection
    $.connection.hub.start()
        .then(init)
        .done(function () {
            //myhub.server.requestTask('suppliersnyc', 'Data Source=192.168.2.14;Initial Catalog=SCMDB_5_3;User ID=mbtscm;Password=mbtscm',
            //    'Data Source=192.168.2.14;Initial Catalog=SCMEP_5_3;User ID=mbtscm;Password=mbtscm', '', '', '', '', 'WAN', 'D:\\Froi\\MyFiles\\', '', '', '');
        });
});
