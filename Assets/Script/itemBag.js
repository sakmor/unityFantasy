var orginPos: Vector3;
var inside: GameObject;
var onDrag: boolean;
var nowPagePos: int = 300;
var nowPage: int = 1;
var change1: int = 440;
var change2: int = 580;
var change3: int = 820;
var change4: int = 1020;
var pageDot_1: GameObject;
var pageDot_2: GameObject;
var pageDot_3: GameObject;
var pageDot_4: GameObject;
var speed: float = 0;
var itemBagON: boolean;
var itemBagcontent = new Array();

function Start() {

    itemBagcontent.push(Vector2(10001, 12));
    itemBagcontent.push(Vector2(10002, 24));
    itemBagcontent.push(Vector2(10003, 32));
    itemBagcontent.push(Vector2(10004, 64));
    itemBagcontent.push(Vector2(10005, 64));
    itemBagcontent.push(Vector2(10017, 64));
    itemBagcontent.push(Vector2(10020, 64));
    itemBagcontent.push(Vector2(10045, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagcontent.push(Vector2(10098, 64));
    itemBagON = false;
    inside = GameObject.Find("inside");
    pageDot_1 = GameObject.Find("pageDot (1)");
    pageDot_2 = GameObject.Find("pageDot (2)");
    pageDot_3 = GameObject.Find("pageDot (3)");
    pageDot_4 = GameObject.Find("pageDot (4)");
    orginPos = inside.transform.position;

    openItemRenew();

}

function openItemRenew() {
    for (var i = 0; i < itemBagcontent.length; i++) {
        var tempVector2: Vector2 = itemBagcontent[i];
        var tempGameObject: GameObject = GameObject.Find("itemBag (" + i + ")");
        tempGameObject.GetComponent. < UI.RawImage > ().texture = Resources.Load('UI/itemicon/' + 'i' + tempVector2.x, Texture);
        tempGameObject = GameObject.Find("itemBag/inside/itemBag (" + i + ")" + "/Text");
        tempGameObject.GetComponent. < UI.Text > ().text = tempVector2.y.ToString();
    }
}

function openItemBag() {
    if (itemBagON) {
        if (this.GetComponent. < RectTransform > ().rect.height != 280) {
            this.GetComponent. < RectTransform > ().sizeDelta = Vector2.MoveTowards(this.GetComponent. < RectTransform > ().sizeDelta, Vector2(960, 280), 20);
        }
    }

}

function closeItemBag() {
    if (!itemBagON) {
        if (this.GetComponent. < RectTransform > ().rect.height != 0) {
            this.GetComponent. < RectTransform > ().sizeDelta = Vector2.MoveTowards(this.GetComponent. < RectTransform > ().sizeDelta, Vector2(960, 0), 20);
        }
    }
}

function LateUpdate() {
    openItemBag();
    closeItemBag();

    //彈力控制
    inside.transform.position = Vector3.MoveTowards(inside.transform.position, Vector3(inside.transform.position.x, nowPagePos, 0), speed);
    speed = Mathf.Abs((inside.transform.position.y - nowPagePos) / 5);

    //點點動畫
    switch (nowPage) {
    case 1:
        pageDot_1.transform.localScale = Vector3.MoveTowards(pageDot_1.transform.localScale, Vector3(0.5, 0.5, 0.5), 0.1);
        pageDot_2.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_3.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_4.transform.localScale = Vector3(0.15, 0.15, 0.15);
        break;
    case 2:
        pageDot_1.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_2.transform.localScale = Vector3.MoveTowards(pageDot_2.transform.localScale, Vector3(0.5, 0.5, 0.5), 0.1);
        pageDot_3.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_4.transform.localScale = Vector3(0.15, 0.15, 0.15);
        break;
    case 3:
        pageDot_1.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_2.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_3.transform.localScale = Vector3.MoveTowards(pageDot_3.transform.localScale, Vector3(0.5, 0.5, 0.5), 0.1);
        pageDot_4.transform.localScale = Vector3(0.15, 0.15, 0.15);
        break;
    case 4:
        pageDot_1.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_2.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_3.transform.localScale = Vector3(0.15, 0.15, 0.15);
        pageDot_4.transform.localScale = Vector3.MoveTowards(pageDot_4.transform.localScale, Vector3(0.5, 0.5, 0.5), 0.1);
        break;
    }
}


function drag(mouseStartPOS: Vector2, myIputPostion: Vector2) {
    speed = 0;
    if (inside.transform.position.y <= change1) {
        nowPagePos = 300;
        nowPage = 1;
    }
    if (inside.transform.position.y > change1 &&
        inside.transform.position.y <= change2) {
        nowPagePos = 560;
        nowPage = 2;
    }
    if (inside.transform.position.y > change2 &&
        inside.transform.position.y <= change3) {
        nowPagePos = 820;
        nowPage = 3;
    }
    if (inside.transform.position.y > change3 &&
        inside.transform.position.y <= change4) {
        nowPagePos = 1080;
        nowPage = 4;
    }

    if ((mouseStartPOS.y - myIputPostion.y) == 0) {
        orginPos = inside.transform.position;
    }

    var goal = orginPos + myIputPostion - mouseStartPOS;
    inside.transform.position = Vector3.MoveTowards(inside.transform.position, Vector3(inside.transform.position.x, goal.y, 0), 300);

}
