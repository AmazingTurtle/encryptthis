function humanFileSize(bytes, si) {
    var thresh = si ? 1000 : 1024;
    if(Math.abs(bytes) < thresh) {
        return bytes + ' B';
    }
    var units = si
        ? ['kB','MB','GB','TB','PB','EB','ZB','YB']
        : ['KiB','MiB','GiB','TiB','PiB','EiB','ZiB','YiB'];
    var u = -1;
    do {
        bytes /= thresh;
        ++u;
    } while(Math.abs(bytes) >= thresh && u < units.length - 1);
    return bytes.toFixed(1)+' '+units[u];
}

var script = {
	init: function () {
		$('.lock-button').click(function (e) {
			e.preventDefault();
			script.togglePasswordOverlay(true);
		});
		$('.continue-button').click(function (e) {
			e.preventDefault();
			var password = $('.password').val();
			$('.password').val('');
			if (api.checkPassword(password))
			{
				script.togglePasswordOverlay(false);
				script.toggleStatusOverlay(true);
				api.toggleLock(password);
			} else {
				$('.password').attr('placeholder', 'Try again...');
			}
		});
		script.updateState();
	},
	updateState: function () {
		var status = api.getStatusData();
		if (status.isLocked) {
			$('.lock-state .glyphicon').removeClass('glyphicon-folder-open').addClass('glyphicon-folder-close');
			$('.lock-state .badge').addClass('badge-locked').text('Locked');
			$('a.lock-button').removeClass('btn-danger').addClass('btn-success').text('Unlock');
		} else {
			$('.lock-state .glyphicon').removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');
			$('.lock-state .badge').removeClass('badge-locked').text('Unlocked');
			$('a.lock-button').removeClass('btn-success').addClass('btn-danger').text('Lock');
		}
		$('.file-info').text(status.totalFiles + " Files in " + status.totalDirectories + " Directories");
		$('.file-info-size').text(humanFileSize(status.totalSize, false));
	},
	update: function (progress) {
		$('.progress-bar-status-report').css('width', progress + '%').attr('aria-valuenow', progress);
		$('.progress-bar-status-report > span').text(progress + '%');
	},
	toggleStatusOverlay: function (visibility) {
		if (visibility) {
			$('.overlay-status').removeClass('inactive');
		} else {
			$('.overlay-status').addClass('inactive');
		}
	},
	togglePasswordOverlay: function (visibility) {
		if (visibility) {
			$('.password').attr('placeholder', 'Password...');
			$('.overlay-password').removeClass('inactive');
		} else {
			$('.overlay-password').addClass('inactive');
		}
	}
}

$(document).ready(function () {
	script.init();
});