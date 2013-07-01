/**Copyright 2013 BLStream, BLStream's Patronage Program Contributors
 *     http://blstream.github.com/UrbanGame/
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package controllers

import play.api.Play
import play.api.mvc._
import play.api.mvc.Results._
import play.api.Play.current
import jp.t2v.lab.play2.auth.AuthConfig
import scala.reflect.classTag
import models.utils._
import models.dal.Bridges._

trait AuthConfigImpl extends AuthConfig {

  type Id = Int

  type User = Operator

  type Authority = Permission

  val idTag = classTag[Id]

  val sessionTimeoutInSeconds: Int = 3600

  def resolveUser(id: Id): Option[User] = findById(id)

  def loginSucceeded(request: RequestHeader) = Redirect(routes.GamesCtrl.myGames)

  def logoutSucceeded(request: RequestHeader) = Redirect(routes.Application.index)

  def authenticationFailed(request: RequestHeader) = {
    request.headers.get("X-Requested-With") match {
      case Some("XMLHttpRequest") => Unauthorized("Authentication failed")
      case _ => Redirect(routes.Application.login)
    }
  }

  def authorizationFailed(req: RequestHeader) = {
    implicit val request = req
    Forbidden(Scalate("permission").render('title -> "Urban Game - Permission denied"))
  }

  def authorize(user: User, authority: Authority): Boolean =
    (user.permission, authority) match {
      case (Administrator, _) => true
      case (NormalUser, NormalUser) => true
      case _ => false
    }

  //override lazy val cookieSecureOption: Boolean = play.api.Play.current.configuration.getBoolean("auth.cookie.secure").getOrElse(true)

}

import jp.t2v.lab.play2.stackc.{RequestAttributeKey, RequestWithAttributes, StackableController}

trait RememberMeElement extends StackableController with AuthConfigImpl {
  self: Controller =>

  val persistentCookieName = "UG_RememberMe"

  val persistentSessionTimeout = 31536000
}

trait EmailConfirmation {
  import com.typesafe.plugin._
  import play.api.i18n._

  private val mail = use[MailerPlugin].email

  def sendSignUpConfirmation(email: String, token: String, request: RequestHeader) = {
    val lan = request.cookies.get("PLAY_LANG").getOrElse(Cookie("PLAY_LANG", Lang.defaultLang.code)).value.toString
    val from = Play.current.configuration.getString("mail.from").getOrElse("")
    val url = "http://" + request.host + "/confirm/" + email + "/" + token
    val msg = """
      <html>
        <h4>""" + Messages("notify.email.signUpConfirm.title")(Lang(lan)) + """</h4>
        <p>""" + Messages("notify.email.signUpConfirm.body")(Lang(lan)) + """</p>
        <p><a href='""" + url + """'>""" + url + """</a></p>
      </html>
    """
    val subject = Messages("email.signUpConfirm.subject")(Lang(lan))

    mail.setSubject(subject)
    mail.addRecipient(email)
    mail.addFrom(from)
    mail.sendHtml(msg)
    //Console.printf(msg)
  }

}