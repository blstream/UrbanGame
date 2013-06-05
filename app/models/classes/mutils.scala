package models.utils

import play.api.libs.json._
import play.api.libs.functional.syntax._
import com.github.nscala_time.time.Imports._
import scala.util.matching.Regex

case class GamesDetails(id: Option[Int], name: String, version: Int = 1, description: String, location: String, 
  operatorId: Int, created: DateTime, updated: DateTime = DateTime.now, startTime: DateTime = DateTime.now, 
  endTime: DateTime, started: Option[DateTime] = None, ended: Option[DateTime] = None, winning: String = "max_points", 
  nWins: Int = 1, difficulty: String = "easy", maxPlayers: Int = 1000000, awards: String, status: String, 
  image: String="games/gameicon.png", tasksNo: Int = 0)
case class TasksDetails(id: Option[Int], gameId: Int, version: Int, name: String, description: String, 
  deadline: DateTime, maxpoints: Int, maxattempts: Int)
case class OperatorsData(id: Option[Int], login: String, pass_hash: String)
case class SkinsDetails(id: Option[Int], gameId: Int, icon: String)

case class GamesList(id: Int, name: String, version: Int, location: String, startTime: DateTime, 
  endTime: DateTime, status: String, image: String, tasksNo: Int)
case class TasksList(id: Int, name: String, version: Int)

object mutils extends mutils {

  def combineDate(date: String, time: String): DateTime = {
    val timep = new Regex("""(\d+):(\d+):?(\d*)\.?(\d*)""")
    val timep(h,m,s,n) = time
    new DateTime(date).withTime(toInt(h), toInt(m), toInt(s), toInt(n))
  }

  def toInt(s: String): Int = try {
    s.toInt
  } catch {
    case e: NumberFormatException => 0
  }
}

trait mutils {

  def combineDate(date: String, time: String): DateTime

  def toInt(s: String): Int

}