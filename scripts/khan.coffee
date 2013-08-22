# Description:
#   Channel Kirk
#
# Dependencies:
#   None
#
# Configuration:
#   None
#
# Commands:
#   ~khan - Yell
#   ~khan \d+ - Yell louder
#
# Author:
#   terite

module.exports = (robot) ->
  robot.hear /^~k(?:ha|ah)n( \d+)?/i, (msg) ->
    length = msg.match[1] || 30
    length = Math.min length, 50
    length = Math.max length, 1
    msg.send "KH#{Array(length + 1).join("A")}N!"