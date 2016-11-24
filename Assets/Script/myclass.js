#pragma strict

 public class Stuff {
         public var strength : float;
         public var bullets : int;
         public var grenades : int;
         public var rockets : int;
         public var fuel : float;
 }

 function Start () {
     var myStuff = Stuff();
     myStuff.strength = 100.0;
     myStuff.bullets = 50;
     myStuff.grenades = 3;
     myStuff.rockets = 1;
     myStuff.fuel = 1000.0;

     Debug.Log(myStuff.strength);
 }
