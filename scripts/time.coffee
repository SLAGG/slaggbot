# Description:
#   What time does null think it should be?
#
# Dependencies:
#   None
#
# Configuration:
#   None
#
# Commands:
#   hubot: null time
#
# Author:
#   terite
#   Nullsoldier

pad = (num, len) ->
  num = '' + num
  while num.length < len
    num = '0' + num

  return num

module.exports = (robot) ->
  robot.hear /null time/i, (msg) ->
    now  = new Date

    hours = now.getHours()
    minutes = now.getMinutes();

    utdt = "UT (up time)"

    if hours > 11
      hours = 23 - hours
      minutes = 60 - minutes
      utdt = "DT (down time)"

    msg.send("#{pad hours, 2}:#{pad minutes, 2} #{utdt}")
